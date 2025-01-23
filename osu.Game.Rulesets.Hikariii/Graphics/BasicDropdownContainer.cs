using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Game.Graphics.Containers;
using osu.Game.Overlays;

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

    [Resolved(canBeNull: true)]
    private OsuGame? game { get; set; }

    protected OsuGame? Game => game;

    [Resolved]
    private OverlayColourProvider colourProvider { get; set; } = null!;

    protected OverlayColourProvider ColourProvider => colourProvider;

    protected virtual float TargetHeight => 200;

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
            new Box
            {
                Name = "Basic Background",
                Colour = ColourProvider.Background5,
                RelativeSizeAxes = Axes.Both,
            },
            new Container
            {
                RelativeSizeAxes = Axes.X,
                Height = TargetHeight,

                Children =
                [
                    BackgroundContainer,
                    ContentContainer,
                ]
            },
            new Box
            {
                Name = "Bottom Bar",
                Height = 5,
                RelativeSizeAxes = Axes.X,
                Colour = ColourProvider.Content2,
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
            }
        ];
    }

    protected override Container<Drawable> Content => ContentContainer;

    protected override void PopIn()
    {
        this.FadeIn()
            .ResizeHeightTo(TargetHeight, 500, Easing.OutQuint);
    }

    protected override void PopOut()
    {
        this.ResizeHeightTo(0, 300, Easing.OutQuint)
            .Then()
            .FadeOut();
    }
}
