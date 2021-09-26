using LLin.Game.Screens.Mvis.Plugins;

namespace Mvis.Plugin.Sandbox
{
    public class SandboxPanelProvider : MvisPluginProvider
    {
        public override MvisPlugin CreatePlugin => new SandboxPanel();
    }
}
