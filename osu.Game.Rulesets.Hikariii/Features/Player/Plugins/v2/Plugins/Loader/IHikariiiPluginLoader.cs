namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Loader;

public interface IHikariiiPluginLoader
{
    /// <summary>
    /// Load plugins from this loader.
    /// </summary>
    /// <returns>An array of loaded <see cref="LLinPluginProvider"/> instances.</returns>
    IHikariiiPluginProvider[] LoadPlugins();
}
