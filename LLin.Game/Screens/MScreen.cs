using osu.Framework.Screens;

namespace LLin.Game.Screens
{
    public abstract class MScreen : Screen
    {
        protected abstract bool AllowCursor { get; set; }
    }
}
