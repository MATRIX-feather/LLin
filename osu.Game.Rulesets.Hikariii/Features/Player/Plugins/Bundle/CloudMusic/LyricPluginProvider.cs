using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.CloudMusic
{
    public class LyricPluginProvider : LLinPluginProvider
    {
        //在这里制定该Provider要提供的插件
        public override LLinPlugin CreatePlugin => new LyricPlugin();
    }
}
