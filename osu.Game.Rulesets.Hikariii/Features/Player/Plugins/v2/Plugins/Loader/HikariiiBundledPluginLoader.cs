using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Bundled.FancyControls;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Bundled.Yasp;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Loader;

public class HikariiiBundledPluginLoader : IHikariiiPluginLoader
{
    public IHikariiiPluginProvider[] LoadPlugins()
    {
        return
        [
            new YaspProvider(),
            new FancyControls()
        ];
    }
}
