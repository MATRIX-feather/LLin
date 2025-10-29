using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.SandboxToPanel
{
    public class SandboxPanelProvider : LLinPluginProvider
    {
        public override LLinPlugin CreatePlugin => new SandboxPlugin();

        public override string Identifier() => "sandbox_to_panel";
    }
}
