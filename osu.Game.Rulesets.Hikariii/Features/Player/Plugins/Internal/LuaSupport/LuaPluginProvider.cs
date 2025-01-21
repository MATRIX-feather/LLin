using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Internal.LuaSupport
{
    public class LuaPluginProvider : LLinPluginProvider
    {
        public override LLinPlugin CreatePlugin => new LuaPlugin();
    }
}
