namespace osu.Game.Rulesets.IGPlayer.Player.Plugins.Bundle.BottomBar
{
    public class BottomBarProvider : LLinPluginProvider
    {
        public override LLinPlugin CreatePlugin => new LegacyBottomBar();
    }
}
