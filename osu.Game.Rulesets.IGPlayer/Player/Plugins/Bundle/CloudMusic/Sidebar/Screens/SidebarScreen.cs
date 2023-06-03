using osu.Framework.Screens;
using osu.Game.Graphics.UserInterface;

namespace osu.Game.Rulesets.IGPlayer.Player.Plugins.Bundle.CloudMusic.Sidebar.Screens
{
    public abstract partial class SidebarScreen : Screen
    {
        public virtual IconButton[] Entries => new IconButton[] { };
    }
}
