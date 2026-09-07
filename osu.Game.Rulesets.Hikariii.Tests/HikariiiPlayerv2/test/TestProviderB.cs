using System;
using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SettingsItems;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Config;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.test;

public class TestProviderB : IHikariiiPluginProvider
{
    public string GetID() => "test-b";

    public IPluginConfigManager CreatePluginConfig(Storage storageAccess)
    {
        return new DummyPluginConfigManager();
    }

    public Type GetPluginConfigType()
    {
        return typeof(DummyPluginConfigManager);
    }

    public PluginDescription GetPluginDescription() => new("NameB", "DescriptionB", ["author1", "author2"]);

    public DrawableHikariiiPlugin CreateDrawablePlugin()
    {
        throw new NotImplementedException();
    }

    public SettingsEntry[] GetSettingsEntries(IPluginConfigManager config) => [];
}
