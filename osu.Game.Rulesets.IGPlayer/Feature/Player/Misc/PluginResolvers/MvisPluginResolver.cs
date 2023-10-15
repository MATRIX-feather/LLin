using System;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Misc.PluginResolvers
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
