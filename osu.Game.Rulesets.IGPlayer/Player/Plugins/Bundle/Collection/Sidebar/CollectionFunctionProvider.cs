using M.Resources.Localisation.LLin.Plugins;
using osu.Framework.Graphics.Sprites;
using osu.Game.Rulesets.IGPlayer.Player.Graphics;
using osu.Game.Rulesets.IGPlayer.Player.Plugins.Types;

namespace osu.Game.Rulesets.IGPlayer.Player.Plugins.Bundle.Collection.Sidebar
{
    public class CollectionFunctionProvider : ButtonWrapper, IPluginFunctionProvider
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
