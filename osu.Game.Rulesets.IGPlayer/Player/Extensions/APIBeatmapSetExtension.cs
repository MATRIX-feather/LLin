using osu.Game.Online.API.Requests.Responses;

namespace osu.Game.Rulesets.IGPlayer.Player.Extensions;

public static class APIBeatmapSetExtension
{
    public static string GetDisplayTitle(this APIBeatmapSet apiSet)
    {
        return string.IsNullOrEmpty(apiSet.TitleUnicode) ? apiSet.Title : apiSet.TitleUnicode;
    }
}
