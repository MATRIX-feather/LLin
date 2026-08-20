using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.BuiltIn.Core;

public class HikariiiCoreConfig : PluginConfigManager<HikariiiCoreConfig.HikariiiSetting>, IPluginConfigManager
{
    public HikariiiCoreConfig(Storage storage)
        : base(storage)
    {
    }

    protected override void InitialiseDefaults()
    {
        SetDefault(HikariiiSetting.BackgroundBlur, 0.2f, 0f, 1f);
        SetDefault(HikariiiSetting.IdleBackgroundDim, 0.8f, 0f, 1f);
        SetDefault(HikariiiSetting.TrianglesInBackground, true);
        SetDefault(HikariiiSetting.EcoMode, false);
        SetDefault(HikariiiSetting.EnableTriangleV2, false);
        SetDefault(HikariiiSetting.EnableFancyIntroOutro, true);
        SetDefault(HikariiiSetting.SettingsMaxWidth, 0.6f, 0.2f, 1f);

        SetDefault(HikariiiSetting.FunctionBarName, "fallback-func-bar");
        SetDefault(HikariiiSetting.AudioPluginName, "osu-audio");

        SetDefault(HikariiiSetting.PlaybackSpeed, 1f, 0.1f, 2.0f);
        SetDefault(HikariiiSetting.AdjustTrackPitch, true);
        SetDefault(HikariiiSetting.NightcoreBeat, false);
    }

    public enum HikariiiSetting
    {
        BackgroundBlur,
        IdleBackgroundDim,
        TrianglesInBackground,
        EcoMode,
        EnableTriangleV2,
        EnableFancyIntroOutro,
        SettingsMaxWidth,

        FunctionBarName,

        AudioPluginName,
        PlaybackSpeed,
        AdjustTrackPitch,
        NightcoreBeat,
    }

    protected override string ConfigName => "core";
}
