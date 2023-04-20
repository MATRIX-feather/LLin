using Mvis.Plugin.Sandbox.RulesetComponents.Configuration;
using Mvis.Plugin.Sandbox.RulesetComponents.UI.Settings;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Overlays.Settings;

namespace Mvis.Plugin.Sandbox.RulesetComponents.Screens.Visualizer.Components.Settings
{
    public partial class VisualizerSection : SandboxSettingsSection
    {
        protected override string HeaderName => "Visualizer";

        [BackgroundDependencyLoader]
        private void load(SandboxRulesetConfigManager config)
        {
            AddRange(new Drawable[]
            {
                new SettingsEnumDropdown<VisualizerLayout>
                {
                    LabelText = "Layout type",
                    Current = config.GetBindable<VisualizerLayout>(SandboxRulesetSetting.VisualizerLayout)
                },
                new LayoutSettingsSubsection()
            });
        }
    }
}
