using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.BottomBar
{
    public class BottomBarProvider : LLinPluginProvider
    {
        public override LLinPlugin CreatePlugin => new LegacyBottomBar();

        public override string Identifier() => "classic_bottom_bar";
    }
}
