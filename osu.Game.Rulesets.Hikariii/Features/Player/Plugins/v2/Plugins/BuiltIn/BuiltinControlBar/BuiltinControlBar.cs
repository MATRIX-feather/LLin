using System;
using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SettingsItems;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Localisation.LLin;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.BuiltIn.BuiltinControlBar;

public class BuiltinControlBar : IHikariiiPluginProvider
{
    public const string ID = "builtin-control-bar";

    public string GetID() => ID;

    public IPluginConfigManager CreatePluginConfig(Storage storageAccess) => new DummyPluginConfigManager();

    public Type GetPluginConfigType() => typeof(DummyPluginConfigManager);

    public PluginDescription GetPluginDescription() => new(LLinPluginNameString.FallbackFunctionBar, "当没有其他底栏能用时会用他", ["mfosu"]);

    public DrawableHikariiiPlugin CreateDrawablePlugin()
    {
        return new DrawableBuiltinControls();
    }

    public SettingsEntry[] GetSettingsEntries(IPluginConfigManager config) => [];
}
