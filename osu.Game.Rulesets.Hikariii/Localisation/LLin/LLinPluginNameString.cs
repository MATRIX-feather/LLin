using osu.Framework.Localisation;

namespace osu.Game.Rulesets.Hikariii.Localisation.LLin;

public class LLinPluginNameString
{
    private const string prefix = @"M.Resources.Localisation.LLin.LLinPluginNameStrings";

    public static LocalisableString FallbackFunctionBar => new TranslatableString(getKey(@"fallback_funtion_bar"), "Fallback control bar");
    public static LocalisableString StandardBottomBar => new TranslatableString(getKey(@"standard_bottom_bar"), "Default control bar");

    public static LocalisableString OsuMusicController => new TranslatableString(getKey(@"osu_music_controller"), "osu! music controller");
    public static LocalisableString CollectionMusicController => new TranslatableString(getKey(@"collection_music_controller"), "Collection music controller");

    public static LocalisableString BackgroundReplay => new TranslatableString(getKey(@"background_replay"), "Background replay");
    public static LocalisableString BackgroundReplayDescription => new TranslatableString(getKey(@"background_replay_description"), "Add replay to background");

    private static string getKey(string key) => $@"{prefix}:{key}";
}
