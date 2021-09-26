using osu.Framework.Allocation;
using osu.Framework.Screens;
using osu.Game.Beatmaps;
using osu.Game.Overlays;
using osu.Game.Screens.Select;

namespace LLin.Game.Screens.Mvis.SongSelect
{
    public class MvisSongSelect : osu.Game.Screens.Select.SongSelect
    {
        public override bool HideOverlaysOnEnter => true;

        [Resolved]
        private MusicController musicController { get; set; }

        private BeatmapSetInfo beatmapSetInfo;

        [BackgroundDependencyLoader]
        private void load()
        {
            beatmapSetInfo = Beatmap.Value.BeatmapSetInfo;

            musicController.CurrentTrack.Looping = true;
        }

        protected override BeatmapDetailArea CreateBeatmapDetailArea() => new MvisBeatmapDetailArea
        {
            SelectCurrentAction = () => OnStart()
        };

        public override bool AllowEditing => false;

        protected override bool OnStart()
        {
            SampleConfirm?.Play();

            if (beatmapSetInfo != Beatmap.Value.BeatmapSetInfo)
                musicController.SeekTo(0);

            this.Exit();

            return true;
        }
    }
}
