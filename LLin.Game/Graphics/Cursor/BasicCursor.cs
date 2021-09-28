using LLin.Game.Screens.Mvis.SideBar.Settings.Items;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Cursor;

namespace LLin.Game.Graphics.Cursor
{
    public class BasicCursor : CursorContainer
    {
        protected override Drawable CreateCursor() => new PlaceHolder
        {
            Width = 15
        };
    }
}
