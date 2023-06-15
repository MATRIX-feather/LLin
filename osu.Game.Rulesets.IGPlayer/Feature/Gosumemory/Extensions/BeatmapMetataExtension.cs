using osu.Game.Beatmaps;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Extensions
{
    public static class BeatmapMetataExtension
    {
        public static string GetTitle(this BeatmapMetadata metadata)
        {
            return string.IsNullOrEmpty(metadata.TitleUnicode)
                ? metadata.Title
                : metadata.TitleUnicode;
        }

        public static string GetArtist(this BeatmapMetadata metadata)
        {
            return string.IsNullOrEmpty(metadata.ArtistUnicode)
                ? metadata.Artist
                : metadata.ArtistUnicode;
        }
    }
}
