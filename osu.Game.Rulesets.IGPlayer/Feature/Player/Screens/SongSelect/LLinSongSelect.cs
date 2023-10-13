#nullable disable

using osu.Framework.Allocation;
using osu.Framework.Screens;
using osu.Game.Overlays;
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
        }

        protected override void ApplyFilterToCarousel(FilterCriteria criteria)
        {
            criteria.RulesetCriteria = null;
            criteria.Ruleset = null;

            base.ApplyFilterToCarousel(criteria);
        }

        protected override BeatmapDetailArea CreateBeatmapDetailArea() => new MvisBeatmapDetailArea
        {
            SelectCurrentAction = () => OnStart()
        };

        public override bool AllowEditing => false;

        protected override bool OnStart()
        {
            SampleConfirm?.Play();
            musicController.SeekTo(0);

            this.Exit();

            return true;
        }
    }
}
