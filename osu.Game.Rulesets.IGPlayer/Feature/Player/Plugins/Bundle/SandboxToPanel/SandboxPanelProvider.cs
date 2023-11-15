using osu.Game.Rulesets.IGPlayer.Feature.Player.Interfaces.Plugins;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.SandboxToPanel
{
    public class SandboxPanelProvider : LLinPluginProvider
    {
        public override LLinPlugin CreatePlugin => new SandboxPlugin();
    }
}
