using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.BuiltIn.Core;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Loader;

public class HikariiiBundledPluginLoader : IHikariiiPluginLoader
{
    public IHikariiiPluginProvider[] LoadPlugins()
    {
        return
        [
            new HikariiiCore()
        ];
    }
}
