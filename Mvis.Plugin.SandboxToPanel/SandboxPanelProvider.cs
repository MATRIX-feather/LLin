using osu.Game.Rulesets.IGPlayer.Player.Plugins;

namespace Mvis.Plugin.Sandbox
{
    public class SandboxPanelProvider : LLinPluginProvider
    {
        public override LLinPlugin CreatePlugin => new SandboxPlugin();
    }
}
