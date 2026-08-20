using System.Collections.Generic;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Loader;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2;

public interface IHikariiiPluginManager
{
    /// <summary>
    /// Get all registered plugin providers.
    /// </summary>
    /// <returns>A dictionary paired with ID and Provider</returns>
    public Dictionary<string, IHikariiiPluginProvider> GetAllPluginProviders();

    /// <summary>
    /// Get plugin provider from the given ID.
    /// </summary>
    /// <param name="id">The ID to use</param>
    /// <returns>An instance of <see cref="LLinPluginProvider"/>, null if not found.</returns>
    public IHikariiiPluginProvider? GetPluginProvider(string id);

    /// <summary>
    /// Try register the given provider with the given ID.
    /// </summary>
    /// <param name="provider">The <see cref="LLinPluginProvider"/> to use.</param>
    /// <param name="id">The ID to use.</param>
    /// <returns>Whether this provider succeed to register. If failed, reason will be print to the log.</returns>
    public bool TryRegisterPlugin(IHikariiiPluginProvider provider, string id);

    /// <summary>
    /// Try to get the config manager for the plugin provider.
    /// </summary>
    /// <param name="provider">The <see cref="LLinPluginProvider"/> to use.</param>
    /// <returns>An instance of <see cref="IPluginConfigManager"/>, null if not found.</returns>
    public C? TryGetPluginConfig<C>(IHikariiiPluginProvider provider) where C : IPluginConfigManager;

    /// <summary>
    /// Load plugins from the given loader.
    /// </summary>
    /// <param name="loader">The <see cref="rejectedProviders"/> to use.</param>
    /// <param name="rejectedProviders">Rejected plugin providers, mostly because of conflict ID, see logs for detail.</param>
    public void LoadFrom(IHikariiiPluginLoader loader, out IHikariiiPluginProvider[] rejectedProviders);
}
