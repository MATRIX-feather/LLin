using System;
using osu.Framework.Configuration;
using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Config
{
    public abstract class PluginConfigManager<TLookup>(Storage storage, IHikariiiPluginProvider pluginProvider) : IniConfigManager<TLookup>(storage), IPluginConfigManager
        where TLookup : struct, Enum
    {
        protected override string Filename => $"hikariii-data/plugins/{pluginProvider.GetID()}/config.ini";
    }
}
