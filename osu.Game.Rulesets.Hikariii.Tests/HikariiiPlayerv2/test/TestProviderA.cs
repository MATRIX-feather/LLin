using System;
using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SettingsItems;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.test;

public class TestProviderA : IHikariiiPluginProvider
{
    public class TestConfigA : IPluginConfigManager
    {
        public void Dispose()
        {
        }
    }

    public string GetID() => "TestA";

    public IPluginConfigManager CreatePluginConfig(Storage storageAccess)
    {
        return new TestConfigA();
    }

    public Type GetPluginConfigType()
    {
        return typeof(TestConfigA);
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
