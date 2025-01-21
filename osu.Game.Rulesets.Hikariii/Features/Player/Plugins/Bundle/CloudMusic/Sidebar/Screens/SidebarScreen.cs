using osu.Framework.Screens;
using osu.Game.Graphics.UserInterface;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.CloudMusic.Sidebar.Screens
{
    public abstract partial class SidebarScreen : Screen
    {
        public virtual IconButton[] Entries => new IconButton[] { };
    }
}
