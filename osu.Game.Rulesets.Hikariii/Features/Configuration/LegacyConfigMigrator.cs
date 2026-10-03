using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Extensions;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.BuiltIn.Core;

namespace osu.Game.Rulesets.Hikariii.Features.Configuration;

public partial class LegacyConfigMigrator : CompositeDrawable
{
    [Resolved]
    private LLinGlobalConfigManager globalConfigManager { get; set; } = null!;

    [Resolved]
    private IHikariiiPluginManager pluginManager { get; set; } = null!;

    [Resolved]
    private Storage storage { get; set; } = null!;

    private DeprecatedMConfigManager mConfigManager = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        this.mConfigManager = new DeprecatedMConfigManager(storage);
    }

    private T fromLegacyConfig<T>(MSetting mSetting)
    {
        return mConfigManager.Get<T>(mSetting);
    }

    public void StartMigrate()
    {
        var hikariiiConfigManager = pluginManager.TryGetPluginConfigOrThrow<HikariiiCoreConfigManager>(pluginManager.GetPluginProviderOrThrow(HikariiiCore.ID));

        globalConfigManager.SetValue(LLinGlobal.AccentRed, fromLegacyConfig<float>(MSetting.MvisInterfaceRed));
        globalConfigManager.SetValue(LLinGlobal.AccentGreen, fromLegacyConfig<float>(MSetting.MvisInterfaceGreen));
        globalConfigManager.SetValue(LLinGlobal.AccentBlue, fromLegacyConfig<float>(MSetting.MvisInterfaceBlue));

        globalConfigManager.SetValue(LLinGlobal.EnableOSDoNotDisturbWhenPlaying, fromLegacyConfig<bool>(MSetting.EnableOSDoNotDisturbWhenPlaying));
        globalConfigManager.SetValue(LLinGlobal.IgnoreMediaControlWhenFocused, fromLegacyConfig<bool>(MSetting.IgnoreMediaControlWhenFocused));
        globalConfigManager.SetValue(LLinGlobal.InjectButtonToNewSongSelect, fromLegacyConfig<bool>(MSetting.InjectButtonToNewSongSelect));
        globalConfigManager.SetValue(LLinGlobal.UsePlatformAccentColor, fromLegacyConfig<bool>(MSetting.UsePlatformAccentColor));

        hikariiiConfigManager.SetValue(HikariiiCoreSetting.AdjustTrackPitch, fromLegacyConfig<bool>(MSetting.MvisAdjustMusicWithFreq));
        hikariiiConfigManager.SetValue(HikariiiCoreSetting.BackgroundBlur, fromLegacyConfig<float>(MSetting.MvisBgBlur));
        hikariiiConfigManager.SetValue(HikariiiCoreSetting.EcoMode, fromLegacyConfig<bool>(MSetting.MvisAutoVSync));
        hikariiiConfigManager.SetValue(HikariiiCoreSetting.EnableFancyIntroOutro, fromLegacyConfig<bool>(MSetting.MvisEnableAdvancedEnterLeaveAnimation));
        hikariiiConfigManager.SetValue(HikariiiCoreSetting.HollowTriangles, fromLegacyConfig<bool>(MSetting.MvisUseTriangleV2));
        hikariiiConfigManager.SetValue(HikariiiCoreSetting.IdleBackgroundDim, fromLegacyConfig<float>(MSetting.MvisIdleBgDim));
        hikariiiConfigManager.SetValue(HikariiiCoreSetting.NightcoreBeat, fromLegacyConfig<bool>(MSetting.MvisEnableNightcoreBeat));
        hikariiiConfigManager.SetValue(HikariiiCoreSetting.PlaybackSpeed, fromLegacyConfig<double>(MSetting.MvisMusicSpeed));
        hikariiiConfigManager.SetValue(HikariiiCoreSetting.SettingsMaxWidth, fromLegacyConfig<float>(MSetting.MvisPlayerSettingsMaxWidth));
        hikariiiConfigManager.SetValue(HikariiiCoreSetting.TrianglesInBackground, fromLegacyConfig<bool>(MSetting.MvisEnableBgTriangles));
    }

    /// <summary>
    /// Move the legacy config file to `mf.ini.bak`, also making
    /// </summary>
    public void MoveLegacyConfigFile()
    {
        storage.Move("mf.ini", "mf.ini.bak");
    }
}
