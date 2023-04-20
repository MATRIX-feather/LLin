using osu.Framework.Graphics.Sprites;

namespace osu.Game.Rulesets.IGPlayer.Player.SideBar
{
    public interface ISidebarContent
    {
        public string Title { get; }

        public IconUsage Icon { get; }
    }
}
