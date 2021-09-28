using LLin.Game.Screens.Mvis;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;
using osuTK;

namespace LLin.Game.Graphics.Cursor
{
    public class LLinTooltipContainer : TooltipContainer
    {
        protected override ITooltip CreateTooltip() => new LLinTooltip();

        public LLinTooltipContainer(CursorContainer cursorContainer)
            : base(cursorContainer)
        {
        }
    }

    public class LLinTooltip : TooltipContainer.Tooltip
    {
        private readonly MSpriteText text = new MSpriteText
        {
            Margin = new MarginPadding { Horizontal = 10, Vertical = 5 }
        };

        public override void SetContent(LocalisableString content)
        {
            text.Text = content;
        }

        public override void Move(Vector2 pos)
        {
            this.MoveTo(pos, 300, Easing.OutQuint);
        }

        private readonly Box bgBox;

        public LLinTooltip()
        {
            AutoSizeAxes = Axes.Both;
            AutoSizeDuration = 300;
            AutoSizeEasing = Easing.OutQuint;

            Masking = true;
            CornerRadius = 5;

            BorderThickness = 3;

            EdgeEffect = new EdgeEffectParameters
            {
                Type = EdgeEffectType.Shadow
            };

            Children = new Drawable[]
            {
                bgBox = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4Extensions.FromHex("#333")
                },
                text
            };
        }

        [Resolved]
        private CustomColourProvider colourProvider { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            colourProvider.HueColour.BindValueChanged(_ => updateColor(), true);
        }

        private void updateColor()
        {
            bgBox.Colour = colourProvider.InActiveColor;
            BorderColour = colourProvider.Highlight1;
        }

        protected override void PopIn() => this.FadeIn(300, Easing.OutQuint);

        protected override void PopOut() => this.Delay(100).FadeOut(300, Easing.OutQuint);
    }
}
