using osu.Framework.Graphics.Sprites;

namespace LLin.Game.Screens.Mvis.SideBar
{
    public interface ISidebarContent
    {
        public string Title { get; }

        public IconUsage Icon { get; }
    }
}
