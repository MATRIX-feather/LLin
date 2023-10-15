using osu.Framework.Localisation;
using osu.Game.Localisation;

namespace osu.Game.Rulesets.IGPlayer.Localisation.LLin.Plugins;

public class StoryboardStrings
{
    private static string prefix = "M.Resources.Localisation.LLin.Plugins.StoryboardStrings";

    private static string getKey(string key) => $@"{prefix}:{key}";

    public static LocalisableString PluginName = GraphicsSettingsStrings.StoryboardVideo;
}
