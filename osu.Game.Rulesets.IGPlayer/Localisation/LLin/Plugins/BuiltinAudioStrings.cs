using osu.Framework.Localisation;
using osu.Game.Localisation;

namespace osu.Game.Rulesets.IGPlayer.Localisation.LLin.Plugins;

public class BuiltinAudioStrings
{
    private static string prefix = "M.Resources.Localisation.LLin.Plugins.BuiltinAudioStrings";

    public static LocalisableString PlaySpeed => getKey(@"play_speed").toTranslatable("Playback speed");

    public static LocalisableString AdjustPitchTitle => getKey("adjust_pitch_title").toTranslatable("Adjust Pitch");

    public static LocalisableString AdjustPitchDesc => getKey("adjust_pitch_desc").toTranslatable("Doesn't support storyboard now.");

    public static LocalisableString NightCoreTitle => getKey("nightcore_beats_title").toTranslatable("Nightcore beats");

    public static LocalisableString NightCoreDesc => getKey("nightcore_beats_desc").toTranslatable("Uguuuuuuuuuuu...");

    public static LocalisableString AudioControlPlugin => getKey("playlist_source").toTranslatable("Playlist source");

    public static LocalisableString PluginName => AudioSettingsStrings.AudioSectionHeader;

    private static string getKey(string key) => $@"{prefix}:{key}";
}

public static class StringExtension
{
    public static LocalisableString toTranslatable(this string key, string fallback) => new TranslatableString(key, fallback);
}
