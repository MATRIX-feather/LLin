using LLin.Game.Graphics.BackgroundOverlays;
using LLin.Game.Graphics.Containers;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;

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

        private readonly TestOverlay settingsOverlay = new TestOverlay();

        [BackgroundDependencyLoader]
        private void load(ScreenContainer screenContainer)
        {
            InternalChildren = new Drawable[]
            {
                new TestToolbarButton(),
                new TestToolbarButton
                {
                    Icon = FontAwesome.Regular.Clone,
                    Action = () => screenContainer.ShowBackgroundOverlay(settingsOverlay)
                }
            };
        }
    }
}
