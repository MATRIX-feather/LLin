using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection
{
    public class CollectionHelperProvider : LLinPluginProvider
    {
        public override LLinPlugin CreatePlugin => new CollectionHelper();

        public override string Identifier() => "collection_helper";
    }
}
