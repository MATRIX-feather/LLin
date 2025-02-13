#nullable disable

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.SandboxToPanel.RulesetComponents.Configuration;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.SandboxToPanel.RulesetComponents.Screens.Visualizer.Components.Layouts.TypeA;
using osu.Game.Screens.Menu;
using osuTK;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.SandboxToPanel.RulesetComponents.Screens.Visualizer.Components.Layouts
{
    public partial class TypeALayout : DrawableVisualizerLayout
    {
        private readonly Bindable<int> radius = new Bindable<int>(350);
        private readonly Bindable<string> colour = new Bindable<string>("#ffffff");
        private readonly Bindable<string> progressColour = new Bindable<string>("#ffffff");
        private readonly BindableBool useMenuVisualizer = new BindableBool();

        private TypeAVisualizerController visualizerController;
        private CircularBeatmapLogo logo;
        private MenuLogoVisualisation logoVisualisation;

        [BackgroundDependencyLoader]
        private void load(SandboxRulesetConfigManager config)
        {
            InternalChildren = new Drawable[]
            {
                visualizerController = new TypeAVisualizerController
                {
                    Position = new Vector2(0.5f),
                },
                logoVisualisation = new MenuLogoVisualisation
                {
                    RelativePositionAxes = Axes.Both,
                    Position = new Vector2(0.5f),
                    Origin = Anchor.Centre
                },
                logo = new CircularBeatmapLogo
                {
                    Position = new Vector2(0.5f),
                    Size = { BindTarget = radius }
                }
            };

            config?.BindWith(SandboxRulesetSetting.Radius, radius);
            config?.BindWith(SandboxRulesetSetting.TypeAColour, colour);
            config?.BindWith(SandboxRulesetSetting.TypeAProgressColour, progressColour);
            config?.BindWith(SandboxRulesetSetting.TypeAUseMenuVisualisation, useMenuVisualizer);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            radius.BindValueChanged(r =>
            {
                if (logoVisualisation != null)
                    logoVisualisation.Size = new Vector2(r.NewValue - 2);

                visualizerController.Size = new Vector2(r.NewValue - 2);
            }, true);

            useMenuVisualizer.BindValueChanged(v =>
            {
                updateVisualizerDisplay(v.NewValue);
            });

            updateVisualizerDisplay(useMenuVisualizer.Value, true);

            colour.BindValueChanged(c => visualizerController.Colour = Colour4.FromHex(c.NewValue), true);
            progressColour.BindValueChanged(c => logo.ProgressColour = Colour4.FromHex(c.NewValue), true);
        }

        private void updateVisualizerDisplay(bool newValue, bool instant = false)
        {
            if (newValue)
            {
                logoVisualisation?.FadeIn(instant ? 0 : 300);
                visualizerController?.FadeOut(instant ? 0 : 300);
            }
            else
            {
                logoVisualisation?.FadeOut(instant ? 0 : 300);
                visualizerController?.FadeIn(instant ? 0 : 300);
            }
        }
    }
}
