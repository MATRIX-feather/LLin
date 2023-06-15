namespace osu.Game.Rulesets.IGPlayer.Player.Plugins.Bundle.SandboxToPanel
{
    public class SandboxPanelProvider : LLinPluginProvider
    {
        public override LLinPlugin CreatePlugin => new SandboxPlugin();
    }
}
