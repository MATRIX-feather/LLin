using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.IGPlayer.Player.Plugins.Bundle.SandboxToPanel.RulesetComponents.Configuration;
using osu.Game.Rulesets.IGPlayer.Player.Plugins.Bundle.SandboxToPanel.RulesetComponents.UI.Settings;
using osuTK;

namespace osu.Game.Rulesets.IGPlayer.Player.Plugins.Bundle.SandboxToPanel.RulesetComponents.Screens.Visualizer.Components.Settings
{
    public partial class ParticleSettings : FillFlowContainer
    {
        private readonly BindableBool showParticles = new BindableBool();

        [BackgroundDependencyLoader]
        private void load(SandboxRulesetConfigManager rulesetConfig)
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            Direction = FillDirection.Vertical;
            Spacing = new Vector2(0, 5);
            Children = new Drawable[]
            {
                new SettingsSlider<int>
                {
                    LabelText = "Particle count",
                    Current = rulesetConfig.GetBindable<int>(SandboxRulesetSetting.ParticleCount),
                    KeyboardStep = 1
                },
                new SettingsSlider<int>
                {
                    LabelText = "Global Speed",
                    Current = rulesetConfig.GetBindable<int>(SandboxRulesetSetting.GlobalSpeed),
                    KeyboardStep = 1
                },
                new SettingsEnumDropdown<ParticlesDirection>
                {
                    LabelText = "Direction",
                    Current = rulesetConfig.GetBindable<ParticlesDirection>(SandboxRulesetSetting.ParticlesDirection)
                },
                new ColourPickerDropdown("Colour", SandboxRulesetSetting.ParticlesColour)
            };

            rulesetConfig.BindWith(SandboxRulesetSetting.ShowParticles, showParticles);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            showParticles.BindValueChanged(s => Alpha = s.NewValue ? 1 : 0, true);
        }
    }
}
