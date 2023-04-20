using osu.Game.Rulesets.IGPlayer.Player.Plugins;

namespace Mvis.Plugin.CollectionSupport
{
    public class CollectionHelperProvider : LLinPluginProvider
    {
        public override LLinPlugin CreatePlugin => new CollectionHelper();
    }
}
