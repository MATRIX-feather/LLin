using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osuTK.Graphics;

namespace LLin.Game.Graphics.BackgroundOverlays.Settings
{
    public class TestOverlayHeader : CompositeDrawable
    {
        public TestOverlayHeader()
        {
            RelativeSizeAxes = Axes.X;
            Height = 34;

            InternalChildren = new Drawable[]
            {
                new MSpriteText
                {
                    Text = "每日一句(bushi)",
                    Font = OsuFont.GetFont(size: 30)
                },
                new Box
                {
                    Height = 2,
                    RelativeSizeAxes = Axes.X,
                    Colour = Color4.White.Opacity(0.6f),
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft
                }
            };
        }
    }
}
