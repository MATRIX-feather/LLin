using System;
using osu.Game.Rulesets.IGPlayer.Player.Plugins;

namespace osu.Game.Rulesets.IGPlayer.Player.Misc.PluginResolvers
{
    [Obsolete("Mvis => LLin")]
    public class MvisPluginResolver : LLinPluginResolver
    {
        public MvisPluginResolver(LLinPluginManager pluginManager)
            : base(pluginManager)
        {
        }
    }
}
