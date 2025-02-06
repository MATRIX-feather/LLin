using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Hikariii.Features.Player.Screens.LLin;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Graphics;

public partial class LLinBottombarPaddingIndicatorMaybe : CompositeDrawable
{
    private Box bottomLine;
    private Box bg;

    [Resolved]
    private CustomColourProvider colourProvider { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren =
        [
            bg = new Box
            {
                Name = "Basic Background",
                Colour = colourProvider.Background5,
                RelativeSizeAxes = Axes.Both,
            },
            bottomLine = new Box
            {
                Name = "Bottom Bar",
                Height = 5,
                RelativeSizeAxes = Axes.X,
                Colour = colourProvider.Content2,
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
            }
        ];

        colourProvider.HueColour.BindValueChanged(v =>
        {
            bg.Colour = colourProvider.Background5;
            bottomLine.Colour = colourProvider.Content2;
        }, true);
    }
}
