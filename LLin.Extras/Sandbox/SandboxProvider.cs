using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SettingsItems;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Config;

namespace LLin.Extras.Sandbox;

public class SandboxProvider : IHikariiiPluginProvider
{
    public string GetID() => "sandbox-to-panel";

    public IPluginConfigManager CreatePluginConfig(Storage storageAccess) => new DummyPluginConfigManager();

    public Type GetPluginConfigType() => typeof(DummyPluginConfigManager);

    public PluginDescription GetPluginDescription() => new
    (
        "Sandbox to panel",
        "sandbox to panel",
        ["EVAST9919 (Author of the sandbox ruleset)", "MATRIX-feather (Author of the sandbox-to-panel plugin)"]
    );

    private readonly SandboxLoader sandboxLoader = new SandboxLoader();

    public DrawableHikariiiPlugin CreateDrawablePlugin() => sandboxLoader.Check() == null ? new DrawableSandboxPlugin() : new DrawableNoDepWarning("Sandbox规则集", "https://github.com/EVAST9919/lazer-sandbox/releases/latest");

    public SettingsEntry[] GetSettingsEntries(IPluginConfigManager config) => [];
}
