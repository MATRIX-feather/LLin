using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Platform;
using osu.Game.Graphics.Cursor;

namespace LLin.Game.Graphics.Cursor
{
    public class LLinCursorContainer : Container, IProvideCursor
    {
        public CursorContainer Cursor { get; }
        public bool ProvidingUserCursor => true;

        [Resolved]
        private GameHost host { get; set; }

        [CanBeNull]
        private SDL2DesktopWindow window => host?.Window as SDL2DesktopWindow;

        public bool ShowSystemCursor
        {
            set
            {
                if (window != null)
                    window.CursorState = value ? CursorState.Hidden : CursorState.Default;
            }
        }

        public LLinCursorContainer()
        {
            Add(Cursor = new BasicCursor());
        }
    }
}
