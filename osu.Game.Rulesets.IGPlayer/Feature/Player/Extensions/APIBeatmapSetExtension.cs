using osu.Game.Online.API.Requests.Responses;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Extensions;

public static class APIBeatmapSetExtension
{
    public static string GetDisplayTitle(this APIBeatmapSet apiSet)
    {
        return string.IsNullOrEmpty(apiSet.TitleUnicode) ? apiSet.Title : apiSet.TitleUnicode;
    }

    public static string GetDisplayArtist(this APIBeatmapSet apiBeatmapSet)
    {
        return string.IsNullOrEmpty(apiBeatmapSet.ArtistUnicode) ? apiBeatmapSet.Artist : apiBeatmapSet.ArtistUnicode;
    }
}
