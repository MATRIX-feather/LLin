using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace LLin.Game.Graphics.Toolbar
{
    public class Toolbar : FillFlowContainer
    {
        public Toolbar()
        {
            RelativeSizeAxes = Axes.X;
            Height = 40;
            Margin = new MarginPadding { Vertical = 40 };
            Padding = new MarginPadding { Horizontal = 50 };

            Direction = FillDirection.Horizontal;
            Anchor = Anchor.TopRight;
            Origin = Anchor.TopRight;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren = new Drawable[]
            {
                new TestToolbarButton()
            };
        }
    }
}
