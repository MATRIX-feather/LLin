using System;
using System.Linq;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.BuiltIn.Core;

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

    public static void AttachAudioAndFunctionbarConfigChanges(this IHikariiiPluginManager manager)
    {
        refreshAvailableAudioAndFunctionControls(manager);
        manager.OnPluginRegister += pair => refreshAvailableAudioAndFunctionControls(manager);
    }

    private static void refreshAvailableAudioAndFunctionControls(IHikariiiPluginManager manager)
    {
        var providers = manager.GetAllPluginProviders().Values;
        var hikariiiCore = manager.GetPluginProviderOrThrow(HikariiiCore.ID) as HikariiiCore;
        IHikariiiPluginProvider[] audios = providers
                                           .Where(p => p.CreateDrawablePlugin() is IProvideAudioControlPlugin)
                                           .ToArray();

        IHikariiiPluginProvider[] funcs = providers
                                          .Where(p => p.CreateDrawablePlugin() is IFunctionBarProvider)
                                          .ToArray();

        hikariiiCore!.UpdateAvailableAudioControllers(audios);
        hikariiiCore.UpdateAvailableFunctionControls(funcs);
    }
}
