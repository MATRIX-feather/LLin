using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Yasp
{
    public class YaspProvider : LLinPluginProvider
    {
        //在这里制定该Provider要提供的插件
        public override LLinPlugin CreatePlugin => new YaspPlugin();
    }
}
