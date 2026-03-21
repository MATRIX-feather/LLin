using System.ComponentModel;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.CloudMusic.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Localisation.LLin.Plugins;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Replay.Config;

public class ReplayConfigManager : PluginConfigManager<ReplaySettings>
{
    public ReplayConfigManager(Storage storage) : base(storage)
    {
    }

    /// <summary>
    /// 在这里初始化默认值, 更多用法请见 <see cref="ConfigManager"/>
    /// </summary>
    protected override void InitialiseDefaults()
    {
        SetDefault(ReplaySettings.EnablePlugin, true);
        SetDefault(ReplaySettings.UseAutoplay, AutoplayPreference.AsAlternative);
        base.InitialiseDefaults();
    }

    protected override string ConfigName => "BackgroundReplay";
}

public enum ReplaySettings
{
    EnablePlugin,
    UseAutoplay,
}

public enum AutoplayPreference
{
    [LocalisableDescription(typeof(BackgroundReplayStrings), nameof(BackgroundReplayStrings.UseAutoplayAsAlternative))]
    AsAlternative,

    [LocalisableDescription(typeof(BackgroundReplayStrings), nameof(BackgroundReplayStrings.AlwaysUseAutoplay))]
    Always,

    [LocalisableDescription(typeof(BackgroundReplayStrings), nameof(BackgroundReplayStrings.NeverUseAutoplay))]
    Never,
}
