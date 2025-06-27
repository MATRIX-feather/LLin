using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Game.Graphics.Containers;
using osu.Game.Rulesets.Hikariii.Features.Player.Screens.LLin;
using osuTK;

namespace osu.Game.Rulesets.Hikariii.Graphics;

public partial class BasicDropdownContainer : OsuFocusedOverlayContainer
{
    public override bool AcceptsFocus => true;
    public override bool RequestsFocus => true;

    protected override void OnFocusLost(FocusLostEvent e)
    {
        if (e.NextFocused != null && e.NextFocused != this)
            Hide();

        base.OnFocusLost(e);
    }

    protected readonly Container ContentContainer;
    protected readonly Container BackgroundContainer;
    private Box backgroundBox;

    [Resolved(canBeNull: true)]
    private OsuGame? game { get; set; }

    protected OsuGame? Game => game;

    [Resolved]
    private CustomColourProvider colourProvider { get; set; } = null!;

    protected CustomColourProvider ColourProvider => colourProvider;

    protected virtual float TargetHeight => 200;

    protected virtual bool UseRelativeHeight => false;

    /// <summary>
    /// 是否在进出动画缩放底BottomLine
    /// </summary>
    protected virtual bool ResizeBarOnAnimation => true;

    protected virtual void OnColorUpdated()
    {
        backgroundBox.Colour = ColourProvider.Background5;
        BottomLine.Colour = ColourProvider.Content2;
    }

    public BasicDropdownContainer()
    {
        Masking = true;

        BackgroundContainer = new Container
        {
            RelativeSizeAxes = Axes.Both,
        };

        ContentContainer = new Container
        {
            RelativeSizeAxes = Axes.Both,
        };

        RelativeSizeAxes = Axes.X;
        Height = 0;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren =
        [
            backgroundBox = new Box
            {
                Name = "Basic Background",
                RelativeSizeAxes = Axes.Both,
            },
            new Container
            {
                RelativeSizeAxes = UseRelativeHeight ? Axes.Both : Axes.X,
                Height = TargetHeight,

                Children =
                [
                    BackgroundContainer,
                    ContentContainer,
                ]
            },
            BottomLine = new Box
            {
                Name = "Bottom Bar",
                Height = 5,
                RelativeSizeAxes = Axes.X,
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
            }
        ];

        colourProvider.HueColour.BindValueChanged(_ => OnColorUpdated());
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        OnColorUpdated();
    }

    protected Box BottomLine { get; set; }

    protected override Container<Drawable> Content => ContentContainer;

    protected override void PopIn()
    {
        this.FadeIn()
            .ResizeHeightTo(TargetHeight, 500, Easing.OutQuint);

        if (ResizeBarOnAnimation)
        {
            var finalAxes = this.ApplyRelativeAxes(this.RelativeSizeAxes, new Vector2(0, this.TargetHeight), FillMode.Fill);

            BottomLine.ResizeHeightTo(finalAxes.Y)
                      .Then()
                      .ResizeHeightTo(5, 500, Easing.OutQuint);
        }
    }

    protected override void PopOut()
    {
        if (ResizeBarOnAnimation)
            BottomLine.ResizeHeightTo(this.DrawHeight, 300, Easing.OutQuint);

        this.ResizeHeightTo(0, 300, Easing.OutQuint)
            .Then()
            .FadeOut();
    }
}
