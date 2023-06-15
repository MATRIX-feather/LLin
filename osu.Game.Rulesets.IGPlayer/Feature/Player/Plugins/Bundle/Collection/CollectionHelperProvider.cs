namespace osu.Game.Rulesets.IGPlayer.Player.Plugins.Bundle.Collection
{
    public class CollectionHelperProvider : LLinPluginProvider
    {
        public override LLinPlugin CreatePlugin => new CollectionHelper();
    }
}
