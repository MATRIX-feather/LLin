using System.Collections.Generic;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.CloudMusic.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.CloudMusic.Helper;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.CloudMusic.Misc;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.CloudMusic.Sidebar;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.CloudMusic.UI;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Types;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.CloudMusic
{
    public partial class LyricPlugin : BindableControlledPlugin
    {
        /// <summary>
        /// 请参阅 <see cref="LLinPlugin.ContentLayer"/>
        /// </summary>
        public override LLinPlugin.ContentLayer Target => LLinPlugin.ContentLayer.Foreground;

        public static bool DisableCloudLookup => false;

        public bool IsContentLoaded => ContentLoaded;

        public override PluginSidebarPage CreateSidebarPage()
            => new LyricSidebarSectionContainer(null); //todo: FIXME null id for lyric sidebar section container

        internal WorkingBeatmap CurrentWorkingBeatmap = null!;
        private readonly LyricLineHandler lrcLine = new LyricLineHandler();

        /// <summary>
        /// 请参阅 <see cref="LLinPlugin.CreateContent()"/>
        /// </summary>
        protected override Drawable CreateContent() => lrcLine;

        public readonly OnlineLyrics LyricProcessor = new OnlineLyrics();

        private List<Lyric>? cachedLyrics;

        public readonly List<Lyric> EmptyLyricList = new List<Lyric>();

        private APILyricResponseRoot? currentResponseRoot;

        [NotNull]
        public List<Lyric> Lyrics
        {
            get => cachedLyrics ?? EmptyLyricList;
            private set => cachedLyrics = value;
        }

        public void ReplaceLyricWith(List<Lyric> newList, bool saveToDisk)
        {
            CurrentStatus.Value = Status.Working;

            Lyrics = newList;

            if (saveToDisk)
                SaveLyricConfigToDisk();

            CurrentStatus.Value = Status.Finish;
        }

        public void GetLyricFor(long id)
        {
            CurrentStatus.Value = Status.Working;
            LyricProcessor.SearchByNeteaseID(id, CurrentWorkingBeatmap, onLyricRequestFinished, onLyricRequestFail);
        }

        private ITrack track = null!;

        public readonly BindableDouble Offset = new BindableDouble
        {
            MaxValue = 60000,
            MinValue = -60000
        };

        private readonly Bindable<bool> autoSave = new Bindable<bool>();

        public readonly Bindable<Status> CurrentStatus = new Bindable<Status>();

        public readonly Bindable<float> TitleSimilarThreshold = new Bindable<float>();

        public LyricPlugin(LLinPluginProvider provider)
            : base(provider)
        {
            Name = "歌词";
            Depth = -1;

            Flags.AddRange([
                PluginFlags.CanDisable
            ]);

            RelativeSizeAxes = Axes.Both;
            Anchor = Origin = Anchor.BottomCentre;
        }

        /// <summary>
        /// 请参阅 <see cref="LLinPlugin.OnContentLoaded(Drawable)"/>
        /// </summary>
        protected override bool OnContentLoaded(Drawable content) => true;

        [Cached]
        public UserDefinitionHelper UserDefinitionHelper { get; private set; } = new UserDefinitionHelper();

        [BackgroundDependencyLoader]
        private void load()
        {
            var config = (LyricConfigManager)Dependencies.Get<LLinPluginManager>().GetConfigManager(Provider.Identifier());

            config.BindWith(LyricSettings.EnablePlugin, Enabled);
            config.BindWith(LyricSettings.SaveLrcWhenFetchFinish, autoSave);
            config.BindWith(LyricSettings.TitleSimilarThreshold, TitleSimilarThreshold);

            AddInternal(LyricProcessor);
            AddInternal(UserDefinitionHelper);

            if (LLin != null)
                LLin.Exiting += onMvisExiting;

            Offset.BindValueChanged(v =>
            {
                if (currentResponseRoot != null)
                    currentResponseRoot.LocalOffset = v.NewValue;
            });

            LLin?.BottomSafeAreaPadding.BindValueChanged(this.onBottomSafeAreaPaddingChanged);
        }

        private void onBottomSafeAreaPaddingChanged(ValueChangedEvent<float> obj)
        {
            var newPadding = new MarginPadding { Bottom = obj.NewValue + 5 };
            this.TransformTo(nameof(Padding), newPadding, 300, Easing.OutQuint);
        }

        private void onMvisExiting()
        {
            this.SaveLyricConfigToDisk(CurrentWorkingBeatmap);
        }

        public void SaveLyricConfigToDisk(WorkingBeatmap? currentBeatmap = null)
        {
            currentBeatmap ??= CurrentWorkingBeatmap;
            LyricProcessor.WriteLrcToFile(currentResponseRoot, currentBeatmap);
        }

        public void RefreshLyric(bool noLocalFile = false)
        {
            CurrentStatus.Value = Status.Working;

            lrcLine.Text = string.Empty;
            lrcLine.TranslatedText = string.Empty;

            Lyrics.Clear();
            currentResponseRoot = null;
            CurrentLine = null;
            Offset.Value = 0d;

            var localLyrics = LyricProcessor.GetLocalLyrics(CurrentWorkingBeatmap);

            if (noLocalFile || localLyrics == null)
            {
                if (UserDefinitionHelper.BeatmapMetaHaveDefinition(CurrentWorkingBeatmap.BeatmapInfo, out long neid))
                    GetLyricFor(neid);
                else if (UserDefinitionHelper.OnlineIDHaveDefinition(CurrentWorkingBeatmap.BeatmapSetInfo.OnlineID, out neid))
                    GetLyricFor(neid);
                else
                    LyricProcessor.Search(SearchOption.From(CurrentWorkingBeatmap, noLocalFile, onLyricRequestFinished, onLyricRequestFail, TitleSimilarThreshold.Value));
            }
            else
            {
                LyricProcessor.State.Value = OnlineLyrics.SearchState.Success;
                onLyricRequestFinished(localLyrics);
            }
        }

        private double targetTime => track.CurrentTime + Offset.Value;

        private void onBeatmapChanged(WorkingBeatmap working)
        {
            if (Disabled.Value) return;

            SaveLyricConfigToDisk(CurrentWorkingBeatmap);

            CurrentWorkingBeatmap = working;
            track = working.Track;

            CurrentStatus.Value = Status.Working;

            RefreshLyric();
        }

        private void onLyricRequestFail(string msg)
        {
            //onLyricRequestFail会在非Update上执行，因此添加Schedule确保不会发生InvalidThreadForMutationException
            Schedule(() =>
            {
                Lyrics.Clear();
                CurrentStatus.Value = Status.Failed;
            });
        }

        private void onLyricRequestFinished(APILyricResponseRoot responseRoot)
        {
            Schedule(() =>
            {
                Offset.Value = responseRoot.LocalOffset;
                currentResponseRoot = responseRoot;

                Lyrics = responseRoot.ToLyricList();

                if (autoSave.Value)
                    SaveLyricConfigToDisk();

                CurrentStatus.Value = Status.Finish;
            });
        }

        public override bool Disable()
        {
            this.MoveToX(-10, 300, Easing.OutQuint).FadeOut(300, Easing.OutQuint);

            return base.Disable();
        }

        public override bool Enable()
        {
            bool result = base.Enable();

            this.MoveToX(0, 300, Easing.OutQuint).FadeIn(300, Easing.OutQuint);

            LLin?.OnBeatmapChanged(onBeatmapChanged, this, true);

            return result;
        }

        protected override bool PostInit() => true;

        private Lyric? currentLine;
        private readonly Lyric emptyLine = new Lyric();

        public Lyric? CurrentLine
        {
            get => currentLine;
            set
            {
                value ??= emptyLine;

                currentLine = value;
            }
        }

        private readonly Lyric defaultLrc = new Lyric();

        protected override void Update()
        {
            base.Update();

            if (ContentLoaded)
            {
                var lrc = Lyrics.FindLast(l => targetTime >= l.Time) ?? defaultLrc;

                if (!lrc.Equals(CurrentLine))
                {
                    lrcLine.Text = lrc.Content;
                    lrcLine.TranslatedText = lrc.TranslatedString;

                    CurrentLine = lrc.GetCopy();
                }
            }
        }

        public enum Status
        {
            Working,
            Failed,
            Finish
        }
    }
}
