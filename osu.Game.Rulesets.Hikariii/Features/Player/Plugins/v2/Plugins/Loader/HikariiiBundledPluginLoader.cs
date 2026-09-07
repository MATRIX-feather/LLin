using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.BuiltIn.BuiltinControlBar;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.BuiltIn.Core;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.BuiltIn.OsuAudio;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Bundled.Yasp;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Loader;

public class HikariiiBundledPluginLoader : IHikariiiPluginLoader
{
    public IHikariiiPluginProvider[] LoadPlugins()
    {
        return
        [
            new HikariiiCore(),
            new OsuAudio(),
            new BuiltinControlBar(),

            new YaspProvider()
        ];
    }
}
