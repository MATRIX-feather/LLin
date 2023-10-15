using osu.Framework.Localisation;
using osu.Game.Rulesets.IGPlayer.Localisation.LLin.Plugins;

namespace osu.Game.Rulesets.IGPlayer.Localisation.LLin;

public class SidebarPageTitles
{
    private const string prefix = "M.Resources.Localisation.LLin.SidebarPageTitles";

    private static string getKey(string key) => $@"{prefix}:{key}";

    public static LocalisableString PlayerSettingsTitle => getKey("player_settings").toTranslatable("Player Settings");

    public static LocalisableString PluginsTitle => getKey("plugins").toTranslatable("Plugins");
}
