using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Collections;
using osu.Game.Database;
using osu.Game.Overlays;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SettingsItems;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SideBar.Settings.Items;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Misc;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection.Chooser;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection.Sidebar;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Types;
using Realms;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection
{
    public partial class CollectionHelper : BindableControlledPlugin, IProvideAudioControlPlugin
    {
        [Resolved]
        private RealmAccess realm { get; set; } = null!;

        private IDisposable? realmSubscription;

        [Resolved]
        private BeatmapManager beatmaps { get; set; } = null!;

        [Resolved]
        private Bindable<WorkingBeatmap> b { get; set; } = null!;

        [Resolved]
        private MusicController controller { get; set; } = null!;

        public Bindable<BeatmapCollection> CurrentCollection = new Bindable<BeatmapCollection>();

        protected override Drawable CreateContent() => new PlaceHolder();

        protected override bool OnContentLoaded(Drawable content) => true;

        protected override bool PostInit() => true;

        public override int Version => 10;

        public override PluginSidebarPage CreateSidebarPage()
            => new CollectionPluginPage(this);

        public override IPluginConfigManager CreateConfigManager(Storage storage)
            => new CollectionHelperConfigManager(storage);

        public override SettingsEntry[] GetSettingEntries(IPluginConfigManager pluginConfigManager)
        {
            var config = (CollectionHelperConfigManager)pluginConfigManager;
            return
            [
                new BooleanSettingsEntry
                {
                    Name = "启用随机播放",
                    Bindable = config.GetBindable<bool>(CollectionSettings.EnableRandom)
                }
            ];
        }

        public CollectionHelper()
        {
            Name = "收藏夹";
            Description = "将收藏夹作为歌单播放音乐!";
            Author = "mf-osu";

            Flags.AddRange(new[]
            {
                LLinPlugin.PluginFlags.CanDisable,
                LLinPlugin.PluginFlags.CanUnload
            });
        }

        private bool trackChangedAfterDisable = true;

        private readonly BindableBool enableRandom = new();

        [BackgroundDependencyLoader]
        private void load()
        {
            var config = (CollectionHelperConfigManager)DependenciesContainer.Get<LLinPluginManager>().GetConfigManager(this);
            config.BindWith(CollectionSettings.EnablePlugin, Enabled);
            config.BindWith(CollectionSettings.EnableRandom, enableRandom);

            b.BindValueChanged(v =>
            {
                beatmapChooser?.OnExternalChoose(v.NewValue);
                if (!IsCurrent) trackChangedAfterDisable = true;
            });

            if (LLin != null)
            {
                LLin.Resuming += UpdateBeatmaps;
                LLin.Exiting += onMvisExiting;
            }

            realmSubscription = realm.RegisterForNotifications(r => r.All<BeatmapCollection>().OrderBy(c => c.Name), onCollectionUpdate);

            enableRandom.BindValueChanged(v =>
            {
                IBeatmapChooser chooser = v.NewValue ? new RandomChooser(beatmaps) : new SequenceChooser(beatmaps);
                selectChooser(chooser);
            }, true);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            CurrentCollection.BindValueChanged(OnCollectionChanged);
        }

        private void onMvisExiting()
        {
        }

        public void Play(WorkingBeatmap b) => changeBeatmap(b);

        public bool NextTrack()
        {
            return changeBeatmap(beatmapChooser?.PickNext());
        }

        public bool PrevTrack()
        {
            return changeBeatmap(beatmapChooser?.PickLast());
        }

        public bool TogglePause()
        {
            try
            {
                if (currentTrack.IsRunning)
                    currentTrack.Stop();
                else
                    currentTrack.Start();
            }
            catch (Exception e)
            {
                Logging.LogError(e, "无法播放音频");
            }

            return true;
        }

        public override bool Disable()
        {
            this.MoveToX(-10, 300, Easing.OutQuint).FadeOut(300, Easing.OutQuint);

            return base.Disable();
        }

        public bool Seek(double position)
        {
            b.Value.Track.Seek(position);

            return true;
        }

        private ITrack currentTrack => b.Value.Track;

        private bool isCurrent;

        public bool IsCurrent
        {
            get => isCurrent;
            set
            {
                if (trackChangedAfterDisable && value)
                {
                    var track = b.Value.Track;

                    track.Completed += onTrackCompleted;
                    trackChangedAfterDisable = false;
                }

                isCurrent = value;
            }
        }

        private bool changeBeatmap(WorkingBeatmap? working)
        {
            if (Disabled.Value) return false;
            if (working == null) return false;

            var track = b.Value.Track;
            track.Completed -= onTrackCompleted;

            b.Disabled = false;
            b.Value = working;
            b.Disabled = IsCurrent;

            // 总是重新开始播放，因为两个谱面可能使用同一个track
            controller.Play(true);
            controller.CurrentTrack.Completed += onTrackCompleted;

            return true;
        }

        private void onTrackCompleted()
        {
            if (IsCurrent) Schedule(() => NextTrack());
        }

        private IBeatmapChooser? beatmapChooser;

        private void selectChooser(IBeatmapChooser chooser)
        {
            Logging.Log($"Now using {chooser} as collection beatmap chooser");

            this.beatmapChooser?.Deactivate();
            beatmapChooser = chooser;
            this.UpdateBeatmaps();
            chooser.OnExternalChoose(b.Value);
        }

        [Resolved]
        private BeatmapHashResolver hashResolver { get; set; } = null!;

        ///<summary>
        ///用来更新<see cref="beatmapList"/>
        ///</summary>
        private void updateBeatmaps(BeatmapCollection collection)
        {
            beatmapChooser?.ClearValidBeatmaps();

            if (collection?.BeatmapMD5Hashes == null) return;

            List<IBeatmapSetInfo> beatmaps = [];

            foreach (string hash in collection.BeatmapMD5Hashes)
            {
                var item = hashResolver.ResolveHash(hash);

                //获取当前BeatmapSet
                var currentSet = item?.BeatmapSet;

                if (currentSet == null)
                {
                    Logging.Log($"{hash}解析到的谱面是null，将不会继续处理此Hash");
                    continue;
                }

                //进行比对，如果beatmapList中不存在，则添加。
                if (!beatmaps.Contains(currentSet))
                    beatmaps.Add(currentSet);
            }

            beatmapChooser?.Activate(beatmaps);
        }

        public void UpdateBeatmaps() => updateBeatmaps(CurrentCollection.Value);

        public List<BeatmapCollection> AvaliableCollections { get; private set; } = new List<BeatmapCollection>();

        public static readonly BeatmapCollection DEFAULT_COLLECTION = new BeatmapCollection("未选择任何收藏夹");

        private void onCollectionUpdate(IRealmCollection<BeatmapCollection> collections, ChangeSet? changes)
        {
            AvaliableCollections = collections.AsEnumerable().Select(c => c).ToList();

            if (CurrentCollection.Value != null)
            {
                var collectionMatch = AvaliableCollections.Find(c => c.ID == CurrentCollection.Value.ID);

                CurrentCollection.Value = collectionMatch ?? DEFAULT_COLLECTION;
            }
        }

        private void OnCollectionChanged(ValueChangedEvent<BeatmapCollection> v)
        {
            updateBeatmaps(CurrentCollection.Value);
        }

        protected override void Dispose(bool isDisposing)
        {
            //collectionManager.Collections.CollectionChanged -= triggerRefresh;

            if (LLin != null)
                LLin.Resuming -= UpdateBeatmaps;

            base.Dispose(isDisposing);

            realmSubscription?.Dispose();
        }
    }
}
