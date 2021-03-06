using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using LLin.Game;
using M.DBus.Tray;
using M.DBus.Utils.Canonical.DBusMenuFlags;
using Mvis.Plugin.CollectionSupport.Config;
using Mvis.Plugin.CollectionSupport.DBus;
using Mvis.Plugin.CollectionSupport.Sidebar;
using osu.Framework;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Audio;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Collections;
using osu.Game.Overlays;
using LLin.Game.Screens.Mvis.Plugins;
using LLin.Game.Screens.Mvis.Plugins.Config;
using LLin.Game.Screens.Mvis.Plugins.Types;
using LLin.Game.Screens.Mvis.SideBar.Settings.Items;

namespace Mvis.Plugin.CollectionSupport
{
    public class CollectionHelper : BindableControlledPlugin, IProvideAudioControlPlugin
    {
        [Resolved]
        private CollectionManager collectionManager { get; set; }

        [Resolved]
        private BeatmapManager beatmaps { get; set; }

        [Resolved]
        private Bindable<WorkingBeatmap> b { get; set; }

        [Resolved]
        private MusicController controller { get; set; }

        private readonly List<BeatmapSetInfo> beatmapList = new List<BeatmapSetInfo>();

        public int CurrentPosition
        {
            get => currentPosition;
            set
            {
                currentPosition = value;

                if (RuntimeInfo.OS == RuntimeInfo.Platform.Linux)
                {
                    dBusObject.Position = value;
                }
            }
        }

        private int currentPosition = -1;
        private int maxCount;
        public Bindable<BeatmapCollection> CurrentCollection = new Bindable<BeatmapCollection>();

        protected override Drawable CreateContent() => new PlaceHolder();

        protected override bool OnContentLoaded(Drawable content) => true;

        protected override bool PostInit() => true;

        public override int Version => 7;

        public override PluginSidebarPage CreateSidebarPage()
            => new CollectionPluginPage(this);

        public override IPluginConfigManager CreateConfigManager(Storage storage)
            => new CollectionHelperConfigManager(storage);

        public CollectionHelper()
        {
            Name = "?????????";
            Description = "????????????????????????????????????!";
            Author = "mf-osu";

            Flags.AddRange(new[]
            {
                PluginFlags.CanDisable,
                PluginFlags.CanUnload
            });
        }

        private bool trackChangedAfterDisable = true;

        [Resolved]
        private LLinGame game { get; set; }

        private CollectionDBusObject dBusObject;

        private readonly SimpleEntry trayEntry = new SimpleEntry
        {
            Label = "???????????????????????????????????????",
            ChildrenDisplay = ChildrenDisplayType.SSubmenu
        };

        [BackgroundDependencyLoader]
        private void load()
        {
            var config = (CollectionHelperConfigManager)DependenciesContainer.Get<MvisPluginManager>().GetConfigManager(this);
            config.BindWith(CollectionSettings.EnablePlugin, Value);
            b.BindValueChanged(v =>
            {
                updateCurrentPosition();
                if (!IsCurrent) trackChangedAfterDisable = true;
            });

            PluginManager.RegisterDBusObject(dBusObject = new CollectionDBusObject());

            if (MvisScreen != null)
            {
                MvisScreen.OnScreenResuming += UpdateBeatmaps;
                MvisScreen.OnScreenExiting += onMvisExiting;
            }
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            CurrentCollection.BindValueChanged(OnCollectionChanged);

            collectionManager.Collections.CollectionChanged += triggerRefresh;
        }

        private void onMvisExiting()
        {
            PluginManager.UnRegisterDBusObject(new CollectionDBusObject());

            if (!Disabled.Value)
                PluginManager.RemoveDBusMenuEntry(trayEntry);

            if (isCurrent)
                MvisScreen.ReleaseAudioControlFrom(this);

            resetDBusMessage();
        }

        public void Play(WorkingBeatmap b) => changeBeatmap(b);

        public void NextTrack() =>
            changeBeatmap(getBeatmap(beatmapList, b.Value, true));

        public void PrevTrack() =>
            changeBeatmap(getBeatmap(beatmapList, b.Value, true, -1));

        public void TogglePause()
        {
            if (drawableTrack.IsRunning)
                drawableTrack.Stop();
            else
                drawableTrack.Start();
        }

        public override bool Disable()
        {
            this.MoveToX(-10, 300, Easing.OutQuint).FadeOut(300, Easing.OutQuint);

            resetDBusMessage();
            PluginManager.RemoveDBusMenuEntry(trayEntry);

            return base.Disable();
        }

        public override bool Enable()
        {
            if (RuntimeInfo.OS == RuntimeInfo.Platform.Linux)
            {
                dBusObject.Position = currentPosition;
                dBusObject.CollectionName = CurrentCollection.Value?.Name.Value ?? "-";
                PluginManager.AddDBusMenuEntry(trayEntry);
            }

            return base.Enable();
        }

        private void resetDBusMessage()
        {
            if (RuntimeInfo.OS == RuntimeInfo.Platform.Linux)
            {
                dBusObject.Position = -1;
                dBusObject.CollectionName = string.Empty;
            }
        }

        public void Seek(double position) => b.Value.Track.Seek(position);

        private DrawableTrack drawableTrack;

        public DrawableTrack GetCurrentTrack() => drawableTrack ??= new DrawableTrack(b.Value.Track);

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
                        if (IsCurrent) Schedule(NextTrack);
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
                if (IsCurrent) Schedule(NextTrack);
            };
            controller.Play();
        }

        /// <summary>
        /// ?????????????????????????????????<see cref="WorkingBeatmap"/>???
        /// </summary>
        /// <returns>???????????????????????????<see cref="WorkingBeatmap"/></returns>
        /// <param name="list">????????????<see cref="BeatmapSetInfo"/>??????</param>
        /// <param name="prevBeatmap">????????????</param>
        /// <param name="updateCurrentPosition">????????????????????????</param>
        /// <param name="displace">????????????????????????1.</param>
        private WorkingBeatmap getBeatmap(List<BeatmapSetInfo> list, WorkingBeatmap prevBeatmap, bool updateCurrentPosition = false, int displace = 1)
        {
            var prevSet = prevBeatmap.BeatmapSetInfo;

            //?????????????????????????????????
            if (updateCurrentPosition)
                CurrentPosition = list.IndexOf(prevSet);

            maxCount = list.Count;

            //?????????????????????????????????
            CurrentPosition += displace;

            //????????????????????????????????????????????????????????????????????????????????????
            if (CurrentPosition >= maxCount || CurrentPosition < 0)
            {
                if (displace > 0) CurrentPosition = 0;
                else CurrentPosition = maxCount - 1;
            }

            //???list???????????????????????????BeatmapSetInfo, ???????????????BeatmapSetInfo???????????????WorkingBeatmap
            //???????????????NewBeatmap
            var newBeatmap = list.Count > 0
                ? beatmaps.GetWorkingBeatmap(list.ElementAt(CurrentPosition).Beatmaps.First())
                : b.Value;
            return newBeatmap;
        }

        ///<summary>
        ///????????????<see cref="beatmapList"/>
        ///</summary>
        private void updateBeatmaps(BeatmapCollection collection)
        {
            //???????????????????????????
            beatmapList.Clear();
            trayEntry.Children.Clear();

            if (collection == null) return;

            foreach (var item in collection.Beatmaps)
            {
                //????????????BeatmapSet
                var currentSet = item.BeatmapSet;
                //?????????????????????beatmapList???????????????????????????
                if (!beatmapList.Contains(currentSet))
                    beatmapList.Add(currentSet);

                if (RuntimeInfo.OS == RuntimeInfo.Platform.Linux)
                {
                    var subEntry = new SimpleEntry
                    {
                        Label = item.BeatmapSet.Metadata.ToRomanisableString().GetPreferred(true),
                        OnActive = () =>
                        {
                            Schedule(() => Play(beatmaps.GetWorkingBeatmap(item)));
                        }
                    };

                    if (trayEntry.Children.All(s => s.Label != subEntry.Label))
                        trayEntry.Children.Add(subEntry);
                }
            }

            if (RuntimeInfo.OS == RuntimeInfo.Platform.Linux)
                dBusObject.CollectionName = collection.Name.Value;

            updateCurrentPosition(true);
            trayEntry.Label = $"????????????{collection.Name}???";
        }

        private SimpleEntry currentSubEntry;

        private void updateCurrentPosition(bool triggerDBusSubmenu = false)
        {
            CurrentPosition = beatmapList.IndexOf(b.Value.BeatmapSetInfo);

            if (RuntimeInfo.OS == RuntimeInfo.Platform.Linux)
            {
                if (currentSubEntry != null)
                    currentSubEntry.ToggleState = 0;

                var targetEntry = trayEntry.Children.FirstOrDefault(s =>
                    s.Label == (b.Value.BeatmapSetInfo.Metadata?.ToRomanisableString().GetPreferred(true) ?? string.Empty));

                if (targetEntry != null)
                    targetEntry.ToggleState = 1;

                currentSubEntry = targetEntry;

                if (triggerDBusSubmenu)
                    trayEntry.TriggerPropertyChangedEvent();
            }
        }

        public void UpdateBeatmaps() => updateBeatmaps(CurrentCollection.Value);

        private void triggerRefresh(object sender, NotifyCollectionChangedEventArgs e)
            => updateBeatmaps(CurrentCollection.Value);

        private void OnCollectionChanged(ValueChangedEvent<BeatmapCollection> v)
        {
            updateBeatmaps(CurrentCollection.Value);
        }

        protected override void Dispose(bool isDisposing)
        {
            if (collectionManager != null)
                collectionManager.Collections.CollectionChanged -= triggerRefresh;

            if (MvisScreen != null) MvisScreen.OnScreenResuming -= UpdateBeatmaps;

            base.Dispose(isDisposing);
        }
    }
}
