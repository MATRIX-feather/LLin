using osu.Game.Rulesets.IGPlayer.Feature.Player.Interfaces.Plugins;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Internal.LuaSupport
{
    public class LuaPluginProvider : LLinPluginProvider
    {
        public override LLinPlugin CreatePlugin => new LuaPlugin();
    }
}
