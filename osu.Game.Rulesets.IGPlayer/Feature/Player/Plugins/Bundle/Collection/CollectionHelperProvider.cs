using osu.Game.Rulesets.IGPlayer.Feature.Player.Interfaces.Plugins;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.Collection
{
    public class CollectionHelperProvider : LLinPluginProvider
    {
        public override LLinPlugin CreatePlugin => new CollectionHelper();
    }
}
