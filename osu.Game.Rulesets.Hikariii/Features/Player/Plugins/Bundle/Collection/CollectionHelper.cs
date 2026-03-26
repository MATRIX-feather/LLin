using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osu.Game.Collections;
using osu.Game.Database;
using osu.Game.Overlays;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SideBar.Settings.Items;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Misc;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection.Chooser;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection.Sidebar;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection.Sorter;
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

        public override PluginSidebarPage CreateSidebarPage()
            => new CollectionPluginPage(this);

        public CollectionHelper(LLinPluginProvider provider)
            : base(provider)
        {
            Name = "收藏夹";

            Flags.AddRange([
                PluginFlags.CanDisable
            ]);
        }

        private bool trackChangedAfterDisable = true;

        private readonly BindableBool enableRandom = new();
        private readonly Bindable<SortMethod> sortMethod = new Bindable<SortMethod>(SortMethod.MostDifficultFirst);

        [BackgroundDependencyLoader]
        private void load()
        {
            var config = (CollectionHelperConfigManager)DependenciesContainer.Get<LLinPluginManager>().GetConfigManager(Provider.Identifier());
            config.BindWith(CollectionSettings.EnablePlugin, Enabled);
            config.BindWith(CollectionSettings.EnableRandom, enableRandom);
            config.BindWith(CollectionSettings.BeatmapSortMethod, sortMethod);

            sortMethod.BindValueChanged(v =>
            {
                beatmapChooser?.SetCandidateSortMethod(v.NewValue);
            });

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

        private void onCollectionUpdate(IRealmCollection<BeatmapCollection> collections, ChangeSet? changes)
        {
            AvaliableCollections = collections.AsEnumerable().Select(c => c).ToList();

            if (CurrentCollection.Value == null) return;

            var collectionMatch = AvaliableCollections.Find(c => c.ID == CurrentCollection.Value.ID);
            CurrentCollection.Value = collectionMatch ?? DEFAULT_COLLECTION;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            CurrentCollection.BindValueChanged(v => updateBeatmaps(v.NewValue));
        }

        private void onMvisExiting()
        {
        }

        public bool Play(BeatmapSetInfo beatmapSetInfo)
        {
            var beatmap = beatmapChooser?.PickFrom(beatmapSetInfo);
            if (beatmap == null || beatmap == LLin?.Beatmap.Value) return false;

            return changeBeatmap(beatmap);
        }

        public void Play(BeatmapInfo b)
        {
            var asWorking = beatmaps.GetWorkingBeatmap(b);
            changeBeatmap(asWorking);
        }

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

        public bool AllowOsuControls { get; } = false;

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

            this.beatmapChooser?.ClearBeatmapCandidates();
            beatmapChooser = chooser;

            chooser.SetBeatmapCandidates(cachedCollectionContent);
            chooser.SetCandidateSortMethod(sortMethod.Value);
            chooser.OnExternalChoose(b.Value);
        }

        [Resolved]
        private BeatmapHashResolver hashResolver { get; set; } = null!;

        private readonly Dictionary<BeatmapSetInfo, List<BeatmapInfo>> cachedCollectionContent = [];

        private readonly List<string> cachedMD5List = [];

        /// <summary>
        /// Extract playable beatmaps from the given beatmap collection.
        /// </summary>
        /// <param name="collection">The beatmap collection to sort</param>
        /// <returns>
        /// A dictionary of BeatmapSet <![CDATA[<->]]> Available Beatmaps
        /// </returns>
        public Dictionary<BeatmapSetInfo, List<BeatmapInfo>> ExtractBeatmaps(BeatmapCollection collection)
        {
            Dictionary<BeatmapSetInfo, List<BeatmapInfo>> beatmapDictionary = new();

            foreach (string hash in collection.BeatmapMD5Hashes)
            {
                var item = hashResolver.ResolveHash(hash);

                if (item?.BeatmapSet == null) continue;

                var existing = beatmapDictionary.GetValueOrDefault(item.BeatmapSet, null);

                if (existing == null)
                {
                    existing = [];
                    beatmapDictionary[item.BeatmapSet] = existing;
                }

                existing.Add(item);
            }

            return beatmapDictionary;
        }

        ///<summary>
        /// Update available beatmaps for the beatmap chooser using the given collection. Won't do anything is the collection is null.
        ///</summary>
        private void updateBeatmaps(BeatmapCollection? collection)
        {
            if (collection?.BeatmapMD5Hashes == null) return;

            if (collection.BeatmapMD5Hashes.SequenceEqual(cachedMD5List))
            {
                //Logging.Log("HashSum is the same, skipping update...");
                return;
            }

            beatmapChooser?.ClearBeatmapCandidates();

            cachedMD5List.Clear();
            cachedMD5List.AddRange(collection.BeatmapMD5Hashes);

            var sortedBeatmaps = ExtractBeatmaps(collection);

            cachedCollectionContent.Clear();
            foreach (var keyValuePair in sortedBeatmaps)
                cachedCollectionContent[keyValuePair.Key] = keyValuePair.Value;

            beatmapChooser?.SetBeatmapCandidates(sortedBeatmaps);
            beatmapChooser?.OnExternalChoose(b.Value);
        }

        public void UpdateBeatmaps() => updateBeatmaps(CurrentCollection.Value);

        public List<BeatmapCollection> AvaliableCollections { get; private set; } = new List<BeatmapCollection>();

        public static readonly BeatmapCollection DEFAULT_COLLECTION = new BeatmapCollection("未选择任何收藏夹");

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
