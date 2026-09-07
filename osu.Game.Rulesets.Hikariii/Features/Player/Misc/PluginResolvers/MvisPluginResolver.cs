using System;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Misc.PluginResolvers
{
    [Obsolete("Mvis => LLin")]
    public class MvisPluginResolver : LLinPluginResolver
    {
        public MvisPluginResolver(LLinPluginManager pluginManager)
            : base(null)
        {
        }
    }
}
