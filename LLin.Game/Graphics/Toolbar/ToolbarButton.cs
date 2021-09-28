using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;
using osuTK.Graphics;

namespace LLin.Game.Graphics.Toolbar
{
    public abstract class ToolbarButton : Button, IHasTooltip
    {
        private readonly Box flashBox;
        public LocalisableString TooltipText { get; set; }

        protected virtual IconUsage Icon { get; set; }

        protected override Container<Drawable> Content => content;

        private readonly Container content = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Masking = true,
            CornerRadius = 7.5f
        };

        protected ToolbarButton()
        {
            RelativeSizeAxes = Axes.Y;
            Width = 40;

            Anchor = Anchor.TopRight;
            Origin = Anchor.TopRight;

            InternalChildren = new Drawable[]
            {
                content
            };

            content.AddRange(new Drawable[]
            {
                new SpriteIcon
                {
                    RelativeSizeAxes = Axes.Both,
                    Icon = Icon,
                    Size = new Vector2(0.6f),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                },
                flashBox = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.White.Opacity(0.2f),
                    Alpha = 0,
                    Depth = float.MinValue
                }
            });
        }

        protected override bool OnHover(HoverEvent e)
        {
            flashBox.FadeIn(300, Easing.OutQuint);
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            flashBox.FadeOut(300, Easing.OutQuint);
            base.OnHoverLost(e);
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            content.ScaleTo(0.6f, 5000, Easing.OutQuint);
            return base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            content.ScaleTo(1, 1000, Easing.OutElastic);
            base.OnMouseUp(e);
        }
    }
}
