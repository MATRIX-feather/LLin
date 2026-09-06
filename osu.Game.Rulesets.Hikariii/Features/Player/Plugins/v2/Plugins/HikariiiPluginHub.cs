using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Extensions;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Loader;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins;

public partial class HikariiiPluginHub : CompositeDrawable, IHikariiiPluginManager
{
    [Resolved]
    private Storage storage { get; set; }

    //region API

    private readonly Dictionary<string, IHikariiiPluginProvider> providers = new();

    public Dictionary<string, IHikariiiPluginProvider> GetAllPluginProviders()
    {
        return new Dictionary<string, IHikariiiPluginProvider>(providers);
    }

    public IHikariiiPluginProvider? GetPluginProvider(string id)
    {
        return providers.GetValueOrDefault(id);
    }

    public Action<(string, IHikariiiPluginProvider)>? InternalDebug_OnNewProviderRegister;

    public bool TryRegisterPlugin(IHikariiiPluginProvider provider, string id)
    {
        if (GetPluginProvider(id) != null)
        {
            Logging.Log($"Already have a provider named '{id}', skipping {provider}#{provider.GetHashCode()}");
            return false;
        }

        if (!provider.GetID().TestHikariiiPluginName())
        {
            Logging.Log($"Test plugin name failed: illegal character in name, or name exceeds the character limit: {provider.GetID()}");
            return false;
        }

        if (!registerConfig(provider))
            return false;

        providers[id] = provider;

        InternalDebug_OnNewProviderRegister?.Invoke((id, provider));
        Logging.Log($"Registered plugin '{id}' for {provider}#{provider.GetHashCode()}");
        return true;
    }

    public C? TryGetPluginConfig<C>(IHikariiiPluginProvider provider)
        where C : IPluginConfigManager
    {
        if (provider is not IHikariiiPluginProvider asGeneric)
            throw new InvalidCastException("这没天理了");

        if (!configManagers.TryGetValue(asGeneric, out var config))
            return (C?)(IPluginConfigManager?)null;

        var expectedType = provider.GetPluginConfigType();
        if (expectedType.IsInstanceOfType(config))
            return (C)config;

        return (C?)(IPluginConfigManager?)null;
    }

    public void LoadFrom(IHikariiiPluginLoader loader, out IHikariiiPluginProvider[] rejectedProviders)
    {
        IHikariiiPluginProvider[] loaded = loader.LoadPlugins();

        // In case I forgot what is this, this is:
        //
        // List<IHikariiiPluginProvider<IPluginConfigManager>> rejected = [];
        //
        // foreach (var hikariiiPluginProvider in providers)
        // {
        //    if (!this.TryRegisterPlugin(hikariiiPluginProvider, hikariiiPluginProvider.GetID()))
        //        rejected.Add(hikariiiPluginProvider);
        // }
        //
        // rejectedProviders = rejected.ToArray();
        rejectedProviders =
        [
            .. loaded.Where(hikariiiPluginProvider => !this.TryRegisterPlugin(hikariiiPluginProvider, hikariiiPluginProvider.GetID()))
        ];
    }

    //endregion API

    private readonly Dictionary<IHikariiiPluginProvider, IPluginConfigManager> configManagers = new();

    private bool registerConfig(IHikariiiPluginProvider provider)
    {
        if (provider is not IHikariiiPluginProvider asGeneric)
        {
            Logging.Log($"Not support provider '{provider}' as it is not an instance of IHikariiiPluginProvider<IPluginConfigManager>");
            return false;
        }

        if (configManagers.ContainsKey(asGeneric))
        {
            Logging.Log($"Trying to register config for {provider}#{provider.GetHashCode()} while they already have one! Not proceeding...");
            return false;
        }

        var config = provider.CreatePluginConfig(storage);
        configManagers[asGeneric] = config;

        Logging.Log($"Registered config for {provider}#{provider.GetHashCode()}");
        return true;
    }
}
