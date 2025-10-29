using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins
{
    public abstract class LLinPluginProvider
    {
        /// <summary>
        /// 要提供的插件
        /// </summary>
        public abstract LLinPlugin CreatePlugin { get; }

        /// <summary>
        /// 此插件的 ID
        /// </summary>
        public abstract string Identifier();
    }
}
