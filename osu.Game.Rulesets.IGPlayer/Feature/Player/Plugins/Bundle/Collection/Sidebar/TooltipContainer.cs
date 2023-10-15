using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Localisation;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.Collection.Sidebar
{
    public partial class TooltipContainer : Container, IHasTooltip
    {
        public LocalisableString TooltipText { get; set; }
    }
}
