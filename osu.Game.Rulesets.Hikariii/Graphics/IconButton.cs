using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osuTK;

namespace osu.Game.Rulesets.Hikariii.Graphics;

public partial class IconButton : OsuAnimatedButton
{
    public IconUsage Icon { get; set; } = FontAwesome.Solid.QuestionCircle;
    public LocalisableString Text { get; set; } = string.Empty;

    [BackgroundDependencyLoader]
    private void load()
    {
        Child = new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,

            Anchor = Anchor.CentreLeft,
            Origin = Anchor.CentreLeft,

            Direction = FillDirection.Horizontal,
            Spacing = new Vector2(7.5f),
            Children =
            [
                new SpriteIcon
                {
                    Icon = Icon,
                    Size = new Vector2(18),
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                },
                new OsuSpriteText
                {
                    Text = Text,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                }
            ]
        };
    }
}
