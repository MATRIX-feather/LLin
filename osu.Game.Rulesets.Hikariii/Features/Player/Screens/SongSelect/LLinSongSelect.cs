#nullable disable

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Screens;
using osu.Game.Beatmaps;
using osu.Game.Overlays;
using osu.Game.Screens.Menu;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Screens.SongSelect
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

        private readonly BindableBool startFromZero = new BindableBool();

        private bool callingStart;

        private void callStart(bool startAtZero)
        {
            callingStart = true;
            //SampleConfirm?.Play();

            if (startAtZero)
                musicController.CurrentTrack.SeekAsync(-1000);

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

        protected override void OnStart()
        {
            callStart(startFromZero.Value);
        }
    }
}
