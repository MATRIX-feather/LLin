using System.Threading;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Timing;
using osu.Game.Beatmaps;
using osu.Game.Localisation;
using osu.Game.Overlays;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Bundled.Storyboard.Storyboard;
using osu.Game.Screens.Play;
using osuTK;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Bundled.Storyboard
{
    ///<summary>
    /// 负责故事版的异步加载功能
    ///</summary>
    public partial class DrawableBackgroundStoryboard : DrawableHikariiiPlugin
    {
        public const float STORYBOARD_FADEIN_DURATION = 750;
        public const float STORYBOARD_FADEOUT_DURATION = STORYBOARD_FADEIN_DURATION / 2;

        ///<summary>
        ///用于内部确定故事版是否已加载
        ///</summary>
        private readonly BindableBool sbLoaded = new BindableBool();

        public readonly BindableBool NeedToHideTriangles = new BindableBool();
        public readonly BindableBool StoryboardReplacesBackground = new BindableBool();

        private BackgroundStoryboard? currentStoryboard;

        public override ContentLayerType ContentLayer => ContentLayerType.Background;

        [Resolved]
        private MusicController music { get; set; } = null!;

        public DrawableBackgroundStoryboard()
        {
            RelativeSizeAxes = Axes.Both;

            Name = "故事版加载器";
        }

        private readonly PlayerLoaderDisclaimer epilepsyWarning = new(PlayerLoaderStrings.EpilepsyWarningTitle, PlayerLoaderStrings.EpilepsyWarningContent)
        {
            Anchor = Anchor.TopCentre,
            Origin = Anchor.TopCentre,
            Margin = new MarginPadding(20),
            Depth = -1,
            Width = 0.5f,
            Scale = new Vector2(2f),
            Alpha = 0
        };

        [BackgroundDependencyLoader]
        private void load()
        {
            //DependenciesContainer.Cache(new OverlayColourProvider(OverlayColourScheme.Lime));

            Player.Exiting += PlayExit;
            Player.Suspending += () => Player.RequestNonBlackBackground(this);
            Player.Resuming += () =>
            {
                if (StoryboardReplacesBackground.Value)
                    Player.RequestBlackBackground(this);
            };
            Player.OnBeatmapChanged(reloadStoryboard, this, true);
            Player.OnSeek += Seek;

            AddInternal(epilepsyWarning);
        }

        protected override void LoadComplete()
        {
            epilepsyWarning.ScaleTo(1);

            if (Player.Beatmap.Value.Beatmap.EpilepsyWarning)
                epilepsyWarning.Show();
            else
                epilepsyWarning.Hide();

            sbLoaded.Value = false;
            NeedToHideTriangles.Value = false;
            StoryboardReplacesBackground.Value = false;
        }

        protected BackgroundStoryboard CreateStoryboard(WorkingBeatmap workingBeatmap)
        {
            DecouplingFramedClock storyboardClock = new DecouplingFramedClock()
            {
                AllowDecoupling = false
            };

            storyboardClock.ChangeSource(Player.AudioClock);

            return new BackgroundStoryboard(workingBeatmap)
            {
                RunningClock = storyboardClock,
                Alpha = 0.1f
            };
        }

        public override void PlayExit()
        {
            Player.RequestNonCleanBackground(this);
            Player.RequestNonBlackBackground(this);

            epilepsyWarning.FinishTransforms();
            epilepsyWarning.Hide();

            exitCurrentStoryboard();
            cancelStoryboardLoad(false);

            this.FadeTo(0.01f, 300, Easing.OutQuint);

            base.PlayExit();
        }

        private void exitCurrentStoryboard()
        {
            if (currentStoryboard != null)
            {
                var sbClock = (DecouplingFramedClock)currentStoryboard.RunningClock;

                double time = sbClock.CurrentTime;
                sbClock.AllowDecoupling = true;
                sbClock.ChangeSource(null);
                sbClock.Seek(time);
                sbClock.Stop();
            }

            currentStoryboard?.FadeTo(0.01f, STORYBOARD_FADEOUT_DURATION, Easing.OutQuint).Then().Expire();
        }

        private void cancelStoryboardLoad(bool expireIfLoaded)
        {
            storyboardCancellationTokenSource?.Cancel();
            storyboardCancellationTokenSource = null;
            Player.HideBusy(this);
        }

        private CancellationTokenSource? storyboardCancellationTokenSource;

        private void reloadStoryboard(WorkingBeatmap newBeatmap)
        {
            exitCurrentStoryboard();
            cancelStoryboardLoad(true);

            storyboardCancellationTokenSource = new CancellationTokenSource();

            Player.ShowBusy(this);
            LoadComponentAsync(CreateStoryboard(newBeatmap), content =>
            {
                this.Add(content);
                currentStoryboard = content;

                Player.HideBusy(this);

                var newStoryboard = (BackgroundStoryboard)content;
                newStoryboard.FadeIn(300, Easing.OutQuint);

                Seek(music.CurrentTrack.CurrentTime);

                sbLoaded.Value = true;
                NeedToHideTriangles.Value = newBeatmap.Storyboard.HasDrawable;
                StoryboardReplacesBackground.Value = newBeatmap.Storyboard.ReplacesBackground;

                if (NeedToHideTriangles.Value) Player.RequestCleanBackground(this);
                else Player.RequestNonCleanBackground(this);

                if (StoryboardReplacesBackground.Value) Player.RequestBlackBackground(this);
                else Player.RequestNonBlackBackground(this);

                if (newBeatmap.Beatmap.EpilepsyWarning)
                    epilepsyWarning.ScaleTo(1.001f, 5000).OnComplete(_ => epilepsyWarning.Hide());
            }, storyboardCancellationTokenSource.Token);
        }

        public void Seek(double position)
        {
        }
    }
}
