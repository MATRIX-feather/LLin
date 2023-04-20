using M.Resources.Localisation.LLin.Plugins;
using osu.Framework.Graphics.Sprites;
using osu.Game.Rulesets.IGPlayer.Player.Plugins;
using osu.Game.Rulesets.IGPlayer.Player.Plugins.Types;

namespace Mvis.Plugin.CollectionSupport.Sidebar
{
    public class CollectionFunctionProvider : FakeButton, IPluginFunctionProvider
    {
        public PluginSidebarPage SourcePage { get; set; }

        public CollectionFunctionProvider(PluginSidebarPage page)
        {
            SourcePage = page;

            Icon = FontAwesome.Solid.Check;
            Description = CollectionStrings.EntryTooltip;
            Type = FunctionType.Plugin;
        }
    }
}
