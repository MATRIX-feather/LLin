using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Game.Overlays;
using osu.Game.Rulesets.Hikariii.Features.Player.Screens.LLin;
using osu.Game.Rulesets.Hikariii.Graphics;
using osu.Game.Tests.Visual;
using osuTK;

namespace osu.Game.Rulesets.Hikariii.Tests;

public partial class TestAboutHikariiiContainer : OsuTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        AddStep("Create focus container", this.createLoopContainer);

        Dependencies.Cache(colourProvider);
    }

    private BasicDropdownContainer? featureDropdownContainer;

    private readonly CustomColourProvider colourProvider = new(OverlayColourScheme.Pink.GetHue());

    private void createLoopContainer()
    {
        featureDropdownContainer?.Hide();
        featureDropdownContainer?.Expire();

        featureDropdownContainer = null;

        LoadComponentAsync(new AboutHikariiiDropdownContainer
        {
        }, container =>
        {
            container.ButtonContainer.AddRange(
            [
                new IconButton
                {
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,

                    Size = new Vector2(150, 30),
                    Icon = FontAwesome.Brands.Accusoft,
                    Text = "打开 Hikariii 播放器",
                    Action = () => { }
                },
                new IconButton
                {
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,

                    Size = new Vector2(150, 30),
                    Icon = FontAwesome.Solid.Cat,
                    Text = "变成猫娘",
                    Action = () => { }
                },
                new IconButton
                {
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,

                    Size = new Vector2(150, 30),
                    Icon = FontAwesome.Solid.MousePointer,
                    Text = "查询PP",
                    Action = () => { }
                }
            ]);

            featureDropdownContainer = container;

            Add(featureDropdownContainer);
            featureDropdownContainer.Show();
        });
    }
}
