using osu.Framework.Graphics.Audio;
using osu.Game.Beatmaps;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Data
{
    public interface ISelfUpdatableFromBeatmap
    {
        public void UpdateMetadata(WorkingBeatmap working);
    }

    public interface ISelfUpdatableFromAudio
    {
        public void UpdateTrack(DrawableTrack track);
    }
}
