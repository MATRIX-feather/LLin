using osu.Framework.Localisation;
using osu.Game.Localisation;

namespace osu.Game.Rulesets.IGPlayer.Localisation.LLin.Plugins;

public class DummyBaseStrings
{
    private static string prefix = "M.Resources.Localisation.LLin.BuiltinBaseSettingsStrings";

    private static string getKey(string key) => $@"{prefix}:{key}";

    public static LocalisableString TabControlPosition = getKey("tab_control_pos").toTranslatable("TabControl Position");

    public static LocalisableString BgDimWhenIdle = getKey("bg_bright_when_idle").toTranslatable("Background dim");

    public static LocalisableString UIColorRed = getKey("ui_color_red").toTranslatable("UI Color (Red)");

    public static LocalisableString UIColorBlue = getKey("ui_color_blue").toTranslatable("UI Color (Blue)");

    public static LocalisableString UIColorGreen = getKey("ui_color_green").toTranslatable("UI Color (Green)");

    public static LocalisableString ProxyOnTop = getKey("proxy_on_top_title").toTranslatable("Put Proxy on top");

    public static LocalisableString ProxyOnTopDesc = getKey("proxy_on_top_desc").toTranslatable("Put all Proxy Layers on top of others");

    public static LocalisableString BackgroundAnimations = getKey("background_animations_title").toTranslatable("Background animations");

    public static LocalisableString BackgroundAnimationsDesc = getKey("background_animations_desc").toTranslatable("Display background animations if possible");

    public static LocalisableString BottomBarPlugin = getKey("bottom_bar_plugin").toTranslatable("Bottom bar");

    public static LocalisableString PowersaveMode = getKey("power_save_mode_title").toTranslatable("Power-save mode");

    public static LocalisableString PowersaveModeDesc = getKey("power_save_mode_desc").toTranslatable("Use V-Sync + Singlethreaded in player, and switch back on exit");

    public static LocalisableString TrianglesV2 = getKey("triangles_v2").toTranslatable("Triangles V2");

    public static LocalisableString MaximumWidthForSettingsPanel = getKey("max_width_for_settings_panel").toTranslatable("Settings area max width");

    public static LocalisableString BgBlur => GameplaySettingsStrings.BackgroundBlur;
}
