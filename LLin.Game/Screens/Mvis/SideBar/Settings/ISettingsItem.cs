using osu.Framework.Bindables;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace LLin.Game.Screens.Mvis.SideBar.Settings
{
    public interface ISettingsItem<T> : IHasTooltip
    {
        public LocalisableString Description { get; set; }
        public IconUsage Icon { get; set; }
        public Bindable<T> Bindable { get; }
    }
}
