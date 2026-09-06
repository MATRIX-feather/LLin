using System;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Extensions;

public static class PluginManagerExtension
{
    public static IHikariiiPluginProvider GetPluginProviderOrThrow(this IHikariiiPluginManager plugins, string id)
    {
        var p = plugins.GetPluginProvider(id);
        return p ?? throw new Exception($"No plugin provider for ID {id}");
    }

    public static C TryGetPluginConfigOrThrow<C>(this IHikariiiPluginManager plugins, string id)
        where C : IPluginConfigManager
    {
        var c = plugins.TryGetPluginConfig<C>(plugins.GetPluginProviderOrThrow(id));
        return c ?? throw new Exception($"No plugin config for ID {id}");
    }

    public static C TryGetPluginConfigOrThrow<C>(this IHikariiiPluginManager plugins, IHikariiiPluginProvider provider)
        where C : IPluginConfigManager
    {
        var c = plugins.TryGetPluginConfig<C>(provider);
        return c ?? throw new Exception($"No plugin config for Class {provider}");
    }
}
