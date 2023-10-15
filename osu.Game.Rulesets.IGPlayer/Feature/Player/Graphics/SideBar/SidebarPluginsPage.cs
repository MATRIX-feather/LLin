using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Graphics.Containers;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Graphics.SideBar.PluginsPage;
using osu.Game.Rulesets.IGPlayer.Localisation.LLin;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Graphics.SideBar
{
    internal partial class SidebarPluginsPage : OsuScrollContainer, ISidebarContent
    {
        public LocalisableString Title => SidebarPageTitles.PluginsTitle;
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
