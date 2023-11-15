using osu.Framework.Graphics.Sprites;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Graphics;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Interfaces.Plugins;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Types;
using osu.Game.Rulesets.IGPlayer.Localisation.LLin.Plugins;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.Collection.Sidebar
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
