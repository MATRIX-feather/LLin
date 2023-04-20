using osu.Game.Rulesets.IGPlayer.Player.Plugins;

namespace Mvis.Plugin.BottomBar
{
    public class BottomBarProvider : LLinPluginProvider
    {
        public override LLinPlugin CreatePlugin => new LegacyBottomBar();
    }
}
