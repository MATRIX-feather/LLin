using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Graphics.SideBar
{
    public interface ISidebarContent
    {
        public LocalisableString Title { get; }

        public IconUsage Icon { get; }
    }
}
