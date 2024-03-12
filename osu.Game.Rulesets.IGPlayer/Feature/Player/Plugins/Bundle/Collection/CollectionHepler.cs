using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Audio;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Collections;
using osu.Game.Database;
using osu.Game.Overlays;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Graphics;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Graphics.SideBar.Settings.Items;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Interfaces.Plugins;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Misc;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.Collection.Config;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.Collection.Sidebar;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.Collection.Utils;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Config;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Types;
using Realms;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.Collection
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

        private readonly List<IBeatmapSetInfo> beatmapList = new List<IBeatmapSetInfo>();

        public int CurrentPosition
        {
            get => currentPosition;
            set => currentPosition = value;
        }

        private int currentPosition = -1;
        private int maxCount;
        public Bindable<BeatmapCollection> CurrentCollection = new Bindable<BeatmapCollection>();

        protected override Drawable CreateContent() => new PlaceHolder();

        protected override bool OnContentLoaded(Drawable content) => true;

        protected override bool PostInit() => true;

        public override int Version => 10;

        public override PluginSidebarPage CreateSidebarPage()
            => new CollectionPluginPage(this);

        public override IPluginConfigManager CreateConfigManager(Storage storage)
            => new CollectionHelperConfigManager(storage);

        public CollectionHelper()
        {
            Name = "收藏夹";
            Description = "将收藏夹作为歌单播放音乐!";
            Author = "mf-osu";

            Flags.AddRange(new[]
            {
                PluginFlags.CanDisable,
                PluginFlags.CanUnload
            });
        }

        private bool trackChangedAfterDisable = true;

        [BackgroundDependencyLoader]
        private void load()
        {
            var config = (CollectionHelperConfigManager)DependenciesContainer.Get<LLinPluginManager>().GetConfigManager(this);
            config.BindWith(CollectionSettings.EnablePlugin, Enabled);
            b.BindValueChanged(v =>
            {
                updateCurrentPosition();
                if (!IsCurrent) trackChangedAfterDisable = true;
            });

            if (LLin != null)
            {
                LLin.Resuming += UpdateBeatmaps;
            }

            realmSubscription = realm.RegisterForNotifications(r => r.All<BeatmapCollection>().OrderBy(c => c.Name), onCollectionChange);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            CurrentCollection.BindValueChanged(OnCollectionChanged);
        }

        public void Play(WorkingBeatmap b) => changeBeatmap(b);

        public bool NextTrack()
        {
            changeBeatmap(getBeatmap(beatmapList, b.Value, true));

            return beatmapList.Count != 0;
        }

        public bool PrevTrack()
        {
            changeBeatmap(getBeatmap(beatmapList, b.Value, true, -1));

            return beatmapList.Count != 0;
        }

        public bool TogglePause()
        {
            try
            {
                if (drawableTrack.IsRunning)
                    drawableTrack.Stop();
                else
                    drawableTrack.Start();
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

        private DrawableTrack drawableTrack = null!;

        private bool isCurrent;

        public bool IsCurrent
        {
            get => isCurrent;
            set
            {
                if (trackChangedAfterDisable && value)
                {
                    drawableTrack = new DrawableTrack(b.Value.Track);
                    drawableTrack.Completed += () =>
                    {
                        if (IsCurrent) Schedule(() => NextTrack());
                    };
                    trackChangedAfterDisable = false;
                }

                isCurrent = value;
            }
        }

        private void changeBeatmap(WorkingBeatmap working)
        {
            if (Disabled.Value) return;

            b.Disabled = false;
            b.Value = working;
            b.Disabled = IsCurrent;
            drawableTrack = new DrawableTrack(b.Value.Track);
            drawableTrack.Completed += () =>
            {
                if (IsCurrent) Schedule(() => NextTrack());
            };
            controller.Play();
        }

        /// <summary>
        /// 用于从列表中获取指定的<see cref="WorkingBeatmap"/>。
        /// </summary>
        /// <returns>根据给定位移得到的<see cref="WorkingBeatmap"/></returns>
        /// <param name="list">要给予的<see cref="BeatmapSetInfo"/>列表</param>
        /// <param name="prevBeatmap">上一张图</param>
        /// <param name="updateCurrentPosition">是否更新当前位置</param>
        /// <param name="displace">位移数值，默认为1.</param>
        private WorkingBeatmap getBeatmap(List<IBeatmapSetInfo> list, WorkingBeatmap prevBeatmap, bool updateCurrentPosition = false, int displace = 1)
        {
            var prevSet = prevBeatmap.BeatmapSetInfo;

            //更新当前位置和最大位置
            if (updateCurrentPosition)
                CurrentPosition = list.IndexOf(prevSet);

            maxCount = list.Count;

            //当前位置往指定位置移动
            CurrentPosition += displace;

            //如果当前位置超过了最大位置或者不在范围内，那么回到第一个
            if (CurrentPosition >= maxCount || CurrentPosition < 0)
            {
                if (displace > 0) CurrentPosition = 0;
                else CurrentPosition = maxCount - 1;
            }

            //从list获取当前位置所在的BeatmapSetInfo, 然后选择该BeatmapSetInfo下的第一个WorkingBeatmap
            //最终赋值给NewBeatmap
            var newBeatmap = list.Count > 0
                ? beatmaps.GetWorkingBeatmap(list.ElementAt(CurrentPosition).Beatmaps.First().AsBeatmapInfo())
                : b.Value;
            return newBeatmap;
        }

        [Resolved]
        private BeatmapHashResolver hashResolver { get; set; } = null!;

        ///<summary>
        ///用来更新<see cref="beatmapList"/>
        ///</summary>
        private void updateBeatmaps(BeatmapCollection collection)
        {
            //清理现有的谱面列表
            beatmapList.Clear();

            if (collection?.BeatmapMD5Hashes == null) return;

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
                if (!beatmapList.Contains(currentSet))
                    beatmapList.Add(currentSet);
            }

            updateCurrentPosition(true);
        }

        private void updateCurrentPosition(bool triggerDBusSubmenu = false)
        {
            CurrentPosition = beatmapList.IndexOf(b.Value.BeatmapSetInfo);
        }

        public void UpdateBeatmaps() => updateBeatmaps(CurrentCollection.Value);

        public List<BeatmapCollection> AvaliableCollections { get; private set; } = new List<BeatmapCollection>();

        public static readonly BeatmapCollection DEFAULT_COLLECTION = new BeatmapCollection("未选择任何收藏夹");

        private void onCollectionChange(IRealmCollection<BeatmapCollection> collections, ChangeSet? changes)
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
