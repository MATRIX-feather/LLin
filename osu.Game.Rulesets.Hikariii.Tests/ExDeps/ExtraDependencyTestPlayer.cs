using osu.Framework.Allocation;
using osu.Game.Graphics.Sprites;
using LLin.Extras;
using LLin.Extras.Sandbox;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Loader;
using osu.Game.Rulesets.Hikariii.Tests.HikariiiPlayerv2;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hikariii.Tests.ExDeps;

public partial class ExtraDependencyTestPlayer : TestSceneSongPlayerScreenBase, IHikariiiPluginLoader
{
    [BackgroundDependencyLoader]
    private void load()
    {
        // Add Resource store
        var loader = new SandboxLoader();

        if (loader.Check() != null)
        {
            Add(
                new OsuSpriteText
                {
                    Text = "Sandbox ruleset not available",
                    Colour = Color4.Red
                });
            return;
        }

        PluginHub.LoadFrom(this, out _);
    }

    public IHikariiiPluginProvider[] LoadPlugins()
    {
        return [new SandboxProvider()];
    }
}
