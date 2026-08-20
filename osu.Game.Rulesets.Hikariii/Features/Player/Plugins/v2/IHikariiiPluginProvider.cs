using System;
using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SettingsItems;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2;

public interface IHikariiiPluginProvider
{
    public string GetID();

    public IPluginConfigManager CreatePluginConfig(Storage storageAccess);
    public Type GetPluginConfigType();

    public PluginDescription GetPluginDescription();

    public DrawableHikariiiPlugin CreateDrawablePlugin();

    // due to knowledge limitations, we can only use IPluginConfigManager at this time.
    public SettingsEntry[] GetSettingsEntries(IPluginConfigManager config);
}
