using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets.Hikariii.Features.Player.Screens.LLin;
using osuTK;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Misc;

public partial class LoadingIndicator : Container
{
    private readonly LoadingSpinner spinner = new(false, false)
    {
        Anchor = Anchor.Centre,
        Origin = Anchor.Centre,
        RelativeSizeAxes = Axes.Both,
        Size = new Vector2(0.55f)
    };

    private readonly Box bgBox = new Box
    {
        RelativeSizeAxes = Axes.Both,
    };

    [Resolved]
    private CustomColourProvider colourProvider { get; set; } = null!;

    private readonly Container content = new Container
    {
        RelativeSizeAxes = Axes.Both,
        Anchor = Anchor.Centre,
        Origin = Anchor.Centre,
        Masking = true,
        CornerRadius = 25f,
        BorderThickness = 4,
    };

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = content;

        content.AddRange(new Drawable[] { bgBox, spinner });

        colourProvider.HueColour.BindValueChanged(v =>
        {
            bgBox.Colour = colourProvider.Dark5;
            content.BorderColour = ColourInfo.GradientVertical(colourProvider.Background3, colourProvider.Background1);
        }, true);
    }

    public override void Show()
    {
        Displaying = true;

        spinner.Show();
        content.ScaleTo(1, 500, Easing.OutQuint);
        this.FadeIn(500, Easing.OutQuint);
    }

    public bool Displaying;

    public override void Hide()
    {
        Displaying = false;

        spinner.Hide();
        content.ScaleTo(0.9f, 500, Easing.OutQuint);
        this.FadeOut(500, Easing.OutQuint);
    }
}
