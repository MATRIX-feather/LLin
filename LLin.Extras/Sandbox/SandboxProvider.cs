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
        ["EVAST9919 (the original ruleset)", "MATRIX-feather (\"port\" to hikariii)"]
    );

    private readonly SandboxLoader sandboxLoader = new SandboxLoader();

    public DrawableHikariiiPlugin CreateDrawablePlugin() => sandboxLoader.Check() == null ? new DrawableSandboxPlugin() : new DrawableNoDepWarning("Sandbox");

    public SettingsEntry[] GetSettingsEntries(IPluginConfigManager config) => [];
}
