namespace osu.Game.Rulesets.IGPlayer.Player.Plugins.Internal.LuaSupport
{
    public class LuaPluginProvider : LLinPluginProvider
    {
        public override LLinPlugin CreatePlugin => new LuaPlugin();
    }
}
