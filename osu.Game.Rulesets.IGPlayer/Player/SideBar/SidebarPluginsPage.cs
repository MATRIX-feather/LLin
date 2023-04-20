using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics.Containers;
using osu.Game.Rulesets.IGPlayer.Player.SideBar.PluginsPage;

namespace osu.Game.Rulesets.IGPlayer.Player.SideBar
{
    internal partial class SidebarPluginsPage : OsuScrollContainer, ISidebarContent
    {
        public string Title => "插件";
        public IconUsage Icon => FontAwesome.Solid.Boxes;

        [BackgroundDependencyLoader]
        private void load()
        {
            ScrollbarVisible = false;
            RelativeSizeAxes = Axes.Both;

            Add(new PluginsSection());
        }
    }
}
