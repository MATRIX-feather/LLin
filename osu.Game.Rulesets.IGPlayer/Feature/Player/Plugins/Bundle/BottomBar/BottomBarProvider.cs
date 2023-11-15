using osu.Game.Rulesets.IGPlayer.Feature.Player.Interfaces.Plugins;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.BottomBar
{
    public class BottomBarProvider : LLinPluginProvider
    {
        public override LLinPlugin CreatePlugin => new LegacyBottomBar();
    }
}
