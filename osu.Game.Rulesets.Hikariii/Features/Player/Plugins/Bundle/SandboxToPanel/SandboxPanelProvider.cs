using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SettingsItems;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.SandboxToPanel.RulesetComponents.Configuration;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Localisation.LLin;
using osu.Game.Rulesets.Hikariii.Localisation.LLin.Plugins;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.SandboxToPanel
{
    public class SandboxPanelProvider : LLinPluginProvider
    {
        public override LLinPlugin CreatePlugin() => new SandboxPlugin(this);

        public override PluginDescription GetDescription() => new("Sandbox2Panel", "提供音频可视化", ["EVAST-9919 (作者)", "mfosu (移植到 LLin)"]);

        public override SandboxRulesetConfigManager CreateConfigManager(Storage storage)
        {
            return new SandboxRulesetConfigManager(storage);
        }

        public override SettingsEntry[] GetSettingEntries(IPluginConfigManager ipcm)
        {
            var config = (SandboxRulesetConfigManager)ipcm;
            return
            [
                new BooleanSettingsEntry
                {
                    Name = LLinGenericStrings.EnablePlugin,
                    Bindable = config.GetBindable<bool>(SandboxRulesetSetting.EnableRulesetPanel)
                },
                new NumberSettingsEntry<float>
                {
                    Name = StpStrings.AlphaOnIdle,
                    Bindable = config.GetBindable<float>(SandboxRulesetSetting.IdleAlpha),
                    DisplayAsPercentage = true
                },
                new BooleanSettingsEntry
                {
                    Name = StpStrings.ShowParticles,
                    Bindable = config.GetBindable<bool>(SandboxRulesetSetting.ShowParticles)
                },
                new NumberSettingsEntry<int>
                {
                    Name = StpStrings.ParticleCount,
                    //////////TransferValueOnCommit = true,
                    Bindable = config.GetBindable<int>(SandboxRulesetSetting.ParticleCount),
                    KeyboardStep = 1,
                },
                new EnumSettingsEntry<VisualizerLayout>
                {
                    Name = StpStrings.VisualizerLayoutType,
                    Bindable = config.GetBindable<VisualizerLayout>(SandboxRulesetSetting.VisualizerLayout)
                },
                new BooleanSettingsEntry
                {
                    Name = StpStrings.SpinningCoverAndVisualizer,
                    Bindable = config.GetBindable<bool>(SandboxRulesetSetting.SpinningCoverAndVisualizer)
                },
                new SeparatorSettingsEntry
                {
                    Name = StpStrings.TypeASettings
                },
                new NumberSettingsEntry<int>
                {
                    Name = StpStrings.Radius,
                    KeyboardStep = 1,
                    Bindable = config.GetBindable<int>(SandboxRulesetSetting.Radius)
                },
                new EnumSettingsEntry<CircularBarType>
                {
                    Name = StpStrings.BarType,
                    Bindable = config.GetBindable<CircularBarType>(SandboxRulesetSetting.CircularBarType)
                },
                new NumberSettingsEntry<int>
                {
                    Name = StpStrings.Rotation,
                    KeyboardStep = 1,
                    Bindable = config.GetBindable<int>(SandboxRulesetSetting.Rotation)
                },
                new NumberSettingsEntry<int>
                {
                    Name = StpStrings.DecayTime,
                    Bindable = config.GetBindable<int>(SandboxRulesetSetting.DecayA),
                    KeyboardStep = 1
                },
                new NumberSettingsEntry<int>
                {
                    Name = StpStrings.HeightMultiplier,
                    Bindable = config.GetBindable<int>(SandboxRulesetSetting.MultiplierA),
                    KeyboardStep = 1
                },
                new BooleanSettingsEntry
                {
                    Name = StpStrings.Symmetry,
                    Bindable = config.GetBindable<bool>(SandboxRulesetSetting.Symmetry)
                },
                new NumberSettingsEntry<int>
                {
                    Name = StpStrings.Smoothness,
                    Bindable = config.GetBindable<int>(SandboxRulesetSetting.SmoothnessA),
                    KeyboardStep = 1
                },
                new NumberSettingsEntry<double>
                {
                    Name = StpStrings.BarWidth,
                    Bindable = config.GetBindable<double>(SandboxRulesetSetting.BarWidthA),
                    KeyboardStep = 0.1f
                },
                new NumberSettingsEntry<int>
                {
                    Name = StpStrings.VisualizerAmount,
                    Bindable = config.GetBindable<int>(SandboxRulesetSetting.VisualizerAmount),
                    KeyboardStep = 1,
                    ////////TransferValueOnCommit = true
                },
                new BooleanSettingsEntry
                {
                    Name = "使用osu!自带的特效显示",
                    Bindable = config.GetBindable<bool>(SandboxRulesetSetting.TypeAUseMenuVisualisation)
                },
                new NumberSettingsEntry<int>
                {
                    Name = StpStrings.BarsPerVisual,
                    Bindable = config.GetBindable<int>(SandboxRulesetSetting.BarsPerVisual),
                    KeyboardStep = 1,
                    ////////TransferValueOnCommit = true
                },
                new SeparatorSettingsEntry
                {
                    Name = StpStrings.TypeBSettings
                },
                new NumberSettingsEntry<int>
                {
                    Name = StpStrings.DecayTime,
                    Bindable = config.GetBindable<int>(SandboxRulesetSetting.DecayB),
                    KeyboardStep = 1
                },
                new NumberSettingsEntry<int>
                {
                    Name = StpStrings.HeightMultiplier,
                    Bindable = config.GetBindable<int>(SandboxRulesetSetting.MultiplierB),
                    KeyboardStep = 1
                },
                new NumberSettingsEntry<int>
                {
                    Name = StpStrings.Smoothness,
                    Bindable = config.GetBindable<int>(SandboxRulesetSetting.SmoothnessB),
                    KeyboardStep = 1
                },
                new NumberSettingsEntry<double>
                {
                    Name = StpStrings.BarWidth,
                    Bindable = config.GetBindable<double>(SandboxRulesetSetting.BarWidthB)
                },
                new NumberSettingsEntry<int>
                {
                    Name = StpStrings.BarCount,
                    Bindable = config.GetBindable<int>(SandboxRulesetSetting.BarCountB)
                },
                new EnumSettingsEntry<LinearBarType>
                {
                    Name = StpStrings.BarType,
                    Bindable = config.GetBindable<LinearBarType>(SandboxRulesetSetting.LinearBarType)
                }
            ];
        }

        public override string Identifier() => "sandbox_to_panel";
    }
}
