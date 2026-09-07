using osu.Framework.Configuration;
using osu.Framework.Platform;

namespace osu.Game.Rulesets.Hikariii.Features.Configuration
{
    public class LLinGlobalConfigManager(Storage storage) : IniConfigManager<LLinGlobal>(storage)
    {
        protected override void InitialiseDefaults()
        {
            SetDefault(LLinGlobal.IgnoreMediaControlWhenFocused, System.OperatingSystem.IsWindows());
            SetDefault(LLinGlobal.EnableOSDoNotDisturbWhenPlaying, true);
            SetDefault(LLinGlobal.UsePlatformAccentColor, true);

            SetDefault(LLinGlobal.InjectButtonToNewSongSelect, true);

            SetDefault(LLinGlobal.AccentRed, value: 0, 0, 255f);
            SetDefault(LLinGlobal.AccentGreen, value: 119f, 0, 255f);
            SetDefault(LLinGlobal.AccentBlue, value: 255f, 0, 255f);

            base.InitialiseDefaults();
        }

        protected override string Filename => "hikariii-data/llin-misc.ini";
    }

    public enum LLinGlobal
    {
        IgnoreMediaControlWhenFocused,
        EnableOSDoNotDisturbWhenPlaying,
        UsePlatformAccentColor,

        // Hikariii player general config
        InjectButtonToNewSongSelect,

        // Global accent color
        AccentRed,
        AccentGreen,
        AccentBlue,
    }
}
