using osu.Framework.Allocation;
using LLin.Extras.Sandbox;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Loader;
using osu.Game.Rulesets.Hikariii.Tests.HikariiiPlayerv2;

namespace osu.Game.Rulesets.Hikariii.Tests.ExDeps;

public partial class ExtraDependencyTestPlayer : TestSceneSongPlayerScreenBase, IHikariiiPluginLoader
{
    [BackgroundDependencyLoader]
    private void load()
    {
        PluginHub.LoadFrom(this, out _);
    }

    public IHikariiiPluginProvider[] LoadPlugins()
    {
        return [new SandboxProvider()];
    }
}
