#nullable disable

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Screens;
using osu.Game.Beatmaps;
using osu.Game.Overlays;
using osu.Game.Screens.Menu;
using osu.Game.Screens.Select;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Screens.SongSelect
{
    public partial class LLinSongSelect : Game.Screens.Select.SongSelect
    {
        public override bool HideOverlaysOnEnter => true;

        [Resolved]
        private MusicController musicController { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            musicController.CurrentTrack.Looping = true;
            Beatmap.BindValueChanged(v =>
            {
                startFromZero.Value = !enteringBeatmap.BeatmapSetInfo.Equals(v.NewValue.BeatmapSetInfo);
            });
        }

        protected override void ApplyFilterToCarousel(FilterCriteria criteria)
        {
            criteria.RulesetCriteria = null;
            criteria.Ruleset = null;

            base.ApplyFilterToCarousel(criteria);
        }

        private readonly BindableBool startFromZero = new BindableBool();

        protected override BeatmapDetailArea CreateBeatmapDetailArea() => new MvisBeatmapDetailArea
        {
            SelectCurrentAction = callStart,
            StartFromZero = { BindTarget = this.startFromZero }
        };

        public override bool AllowEditing => false;

        private bool callingStart;

        private void callStart(bool startAtZero)
        {
            callingStart = true;
            SampleConfirm?.Play();

            if (startAtZero)
                musicController.SeekTo(0);

            this.Exit();
        }

        private WorkingBeatmap enteringBeatmap;
        private double enteringPosition;

        public override void OnEntering(ScreenTransitionEvent e)
        {
            this.enteringBeatmap = Beatmap.Value;
            enteringPosition = musicController.CurrentTrack.CurrentTime;

            base.OnEntering(e);
        }

        public override bool OnExiting(ScreenExitEvent e)
        {
            if (callingStart || enteringBeatmap == null) return base.OnExiting(e);

            Beatmap.Value = enteringBeatmap;
            musicController.SeekTo(enteringPosition);
            musicController.Stop(true);

            return base.OnExiting(e);
        }

        protected override void LogoArriving(OsuLogo logo, bool resuming)
        {
            base.LogoArriving(logo, resuming);
            logo.ScaleTo(0);
        }

        protected override bool OnStart()
        {
            callStart(startFromZero.Value);
            return true;
        }
    }
}
