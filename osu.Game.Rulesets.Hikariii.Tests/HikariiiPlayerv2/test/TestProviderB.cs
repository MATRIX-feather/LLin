using System;
using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SettingsItems;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.test;

public class TestProviderB : IHikariiiPluginProvider
{
    public class TestConfigB : IPluginConfigManager
    {
        public void Dispose()
        {
        }
    }

    public string GetID() => "TestB";

    public IPluginConfigManager CreatePluginConfig(Storage storageAccess)
    {
        return new TestConfigB();
    }

    public Type GetPluginConfigType()
    {
        return typeof(TestConfigB);
    }

    public PluginDescription GetPluginDescription()
    {
        throw new NotImplementedException();
    }

    public DrawableHikariiiPlugin CreateDrawablePlugin()
    {
        throw new NotImplementedException();
    }

    public SettingsEntry[] GetSettingsEntries(IPluginConfigManager config)
    {
        throw new NotImplementedException();
    }
}
