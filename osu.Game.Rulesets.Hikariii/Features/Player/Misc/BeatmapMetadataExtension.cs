using osu.Framework.Localisation;
using osu.Game.Beatmaps;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Misc
{
    public static class BeatmapMetadataExtension
    {
        public static RomanisableString GetTitleRomanisable(this BeatmapMetadata metadata)
        {
            return new RomanisableString(metadata.TitleUnicode, metadata.Title);
        }

        public static RomanisableString GetArtistRomanisable(this BeatmapMetadata metadata)
        {
            return new RomanisableString(metadata.ArtistUnicode, metadata.Artist);
        }

        public static (string Title, bool IsUnicode) GetTitle(this BeatmapMetadata metadata)
        {
            return string.IsNullOrEmpty(metadata.TitleUnicode)
                ? (metadata.Title, false)
                : (metadata.TitleUnicode, true);
        }

        public static string GetArtist(this BeatmapMetadata metadata)
        {
            return string.IsNullOrEmpty(metadata.ArtistUnicode)
                ? metadata.Artist
                : metadata.ArtistUnicode;
        }
    }
}
