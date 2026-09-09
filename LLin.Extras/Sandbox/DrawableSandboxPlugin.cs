using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins;
using osu.Game.Rulesets.Sandbox.Screens.Visualizer.Components;
using osu.Game.Rulesets.Sandbox.Screens.Visualizer.Components.Settings;
using osu.Game.Rulesets.Sandbox.UI.Settings;
using osu.Game.Rulesets.UI;

namespace LLin.Extras.Sandbox;

public partial class DrawableSandboxPlugin : DrawableHikariiiPlugin
{
    private DependencyContainer dependencies = null!;
    private SandboxSettings settingsContainer = null!;

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => dependencies = new DependencyContainer(parent);

    [BackgroundDependencyLoader]
    private void load(IRulesetStore rulesetStore)
    {
        RelativeSizeAxes = Axes.Both;

        var ruleset = rulesetStore.GetRuleset("sandbox")?.CreateInstance() ?? throw new NullDependencyException("Ruleset object not found");

        Children =
        [
            new DrawableRulesetDependenciesProvidingContainer(ruleset)
            {
                RelativeSizeAxes = Axes.Both,
                Children =
                [
                    new Particles(),
                    new LayoutController(),
                    settingsContainer = new SandboxSettings
                    {
                        Anchor = Anchor.TopLeft,
                        Origin = Anchor.TopLeft,

                        Sections =
                        [
                            new BackgroundSection(),
                            new VisualizerSection()
                        ]
                    }
                ]
            }
        ];
    }

    protected override void UpdateAfterChildren()
    {
        base.UpdateAfterChildren();
    }
}
