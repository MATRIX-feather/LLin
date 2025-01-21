using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.SandboxToPanel.RulesetComponents.Screens.Visualizer.Components.Layouts.TypeB;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.SandboxToPanel.RulesetComponents.Screens.Visualizer.Components.Layouts
{
    public partial class TypeBLayout : DrawableVisualizerLayout
    {
        public TypeBLayout()
        {
            AddInternal(new TypeBVisualizerController());
        }
    }
}
