using System;
using osu.Framework.Allocation;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Loader;
using osu.Game.Rulesets.Hikariii.Features.Player.Screens.LLin;
using osu.Game.Rulesets.Hikariii.Tests.HikariiiPlayerv2.test;

namespace osu.Game.Rulesets.Hikariii.Tests.HikariiiPlayerv2;

public partial class TestPlayerPluginEnableDisable : TestSceneSongPlayerScreenBase, IHikariiiPluginLoader
{
    [BackgroundDependencyLoader]
    private void load()
    {
        PluginHub.LoadFrom(this, out _);

        AddStep("Try enable plugin", () => runIfPlayerPresent(player => player.SessionPluginManager.EnablePlugin("test-a")));
        AddStep("Try disable plugin", () => runIfPlayerPresent(player => player.SessionPluginManager.DisablePlugin("test-a")));
    }

    private void runIfPlayerPresent(Action<LLinScreen> action)
    {
        if (CurrentLLin != null)
            action.Invoke(CurrentLLin);
    }

    public IHikariiiPluginProvider[] LoadPlugins()
    {
        return [new TestProviderA()];
    }
}
