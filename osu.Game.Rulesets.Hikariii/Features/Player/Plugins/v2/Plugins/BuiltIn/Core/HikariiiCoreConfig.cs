using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Config;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.BuiltIn.Core
{
    public class HikariiiCoreConfigManager(Storage storage, IHikariiiPluginProvider provider) : PluginConfigManager<HikariiiCoreSetting>(storage, provider)
    {
        protected override void InitialiseDefaults()
        {
            SetDefault(HikariiiCoreSetting.BackgroundBlur, 0.2f, 0f, 1f);
            SetDefault(HikariiiCoreSetting.IdleBackgroundDim, 0.8f, 0f, 1f);
            SetDefault(HikariiiCoreSetting.TrianglesInBackground, true);
            SetDefault(HikariiiCoreSetting.EcoMode, false);
            SetDefault(HikariiiCoreSetting.EnableFancyIntroOutro, true);
            SetDefault(HikariiiCoreSetting.SettingsMaxWidth, 0.6f, 0.2f, 1f);

            SetDefault(HikariiiCoreSetting.FunctionBarName, "fallback-func-bar");
            SetDefault(HikariiiCoreSetting.AudioPluginName, "osu-audio");

            SetDefault(HikariiiCoreSetting.PlaybackSpeed, 1d, 0.1d, 2.0d);
            SetDefault(HikariiiCoreSetting.AdjustTrackPitch, true);
            SetDefault(HikariiiCoreSetting.NightcoreBeat, false);

            SetDefault(HikariiiCoreSetting.FancyHikariiiLoader, true);
            SetDefault(HikariiiCoreSetting.HollowTriangles, false);

            SetDefault(HikariiiCoreSetting.EnabledPlugins, $"{HikariiiCore.ID}");
        }
    }

    public enum HikariiiCoreSetting
    {
        BackgroundBlur,
        IdleBackgroundDim,
        TrianglesInBackground,
        EcoMode,
        EnableFancyIntroOutro,
        SettingsMaxWidth,

        FunctionBarName,

        AudioPluginName,
        PlaybackSpeed,
        AdjustTrackPitch,
        NightcoreBeat,

        FancyHikariiiLoader,
        HollowTriangles,

        EnabledPlugins
    }
}

