#nullable disable

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;
using osu.Game.Overlays;
using osu.Game.Screens.Menu;
using osu.Game.Screens.Select;

namespace osu.Game.Rulesets.IGPlayer.Player.Screens.SongSelect
{
    public partial class LLinSongSelect : Game.Screens.Select.SongSelect
    {
        public override bool HideOverlaysOnEnter => true;

        [Resolved]
        private MusicController musicController { get; set; }

        protected override void LogoArriving(OsuLogo logo, bool resuming)
        {
            base.LogoArriving(logo, resuming);
            logo.ScaleTo(0);
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            musicController.CurrentTrack.Looping = true;
        }

        protected override void ApplyFilterToCarousel(FilterCriteria criteria)
        {
            criteria.RulesetCriteria = null;
            criteria.Ruleset = null;

            base.ApplyFilterToCarousel(criteria);
        }

        protected override BeatmapDetailArea CreateBeatmapDetailArea() => new MvisBeatmapDetailArea
        {
            SelectCurrentAction = callStart
        };

        public override bool AllowEditing => false;

        private void callStart(bool startAtZero)
        {
            SampleConfirm?.Play();

            if (startAtZero)
                musicController.SeekTo(0);

            this.Exit();
        }

        protected override bool OnStart()
        {
            callStart(true);
            return true;
        }
    }
}
