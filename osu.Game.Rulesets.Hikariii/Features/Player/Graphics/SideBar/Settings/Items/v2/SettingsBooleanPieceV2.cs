using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Rulesets.Hikariii.Features.Player.Screens.LLin;
using osuTK;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SideBar.Settings.Items.v2;

public partial class SettingsBooleanPieceV2 : Container
{
    public virtual LocalisableString Title { get; set; } = "SettingsItemName";
    public virtual LocalisableString Description { get; set; } = "SettingsItemDescription";
    public virtual IconUsage Icon { get; set; } = FontAwesome.Solid.ToggleOn;

    [Resolved]
    private CustomColourProvider colourProvider { get; set; }

    private Box backgroundBox = null!;
    private Box iconBackground = null!;
    private SpriteIcon icon = null!;

    //region boolean piece

    public virtual IBindable<bool> Bindable { get; set; } = new Bindable<bool>();

    //endregion

    [BackgroundDependencyLoader]
    private void load()
    {
        Masking = true;
        CornerRadius = 5;
        BorderThickness = 4;
        AutoSizeAxes = Axes.Y;
        Width = 180;
        //Shear = new Vector2(ShearStrength, 0f);

        Children =
        [
            backgroundBox = new Box
            {
                RelativeSizeAxes = Axes.Both
            },
            new GridContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding(4),

                RowDimensions =
                [
                    new Dimension(GridSizeMode.AutoSize),
                    new Dimension()
                ],

                Content = new[]
                {
                    new Drawable[]
                    {
                        new Container
                        {
                            Name = "Icon Container",
                            AutoSizeAxes = Axes.Both,

                            Masking = true,
                            CornerRadius = 5,

                            Children =
                            [
                                iconBackground = new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Width = 0.95f
                                },
                                icon = new SpriteIcon
                                {
                                    Size = new Vector2(28f),
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Margin = new MarginPadding(8),
                                    Icon = this.Icon,
                                }
                            ]
                        },
                        new Container
                        {
                            Name = "Content Container",
                            RelativeSizeAxes = Axes.Both,
                            Children =
                            [
                                new SpriteText
                                {
                                    Text = "Text TBD"
                                }
                            ]
                        }
                    }
                }
            }
        ]; // 💀

        colourProvider.HueColour.BindValueChanged(this.onAccentColorChanged, true);
    }

    private void onAccentColorChanged(ValueChangedEvent<float> newHue)
    {
        backgroundBox.Colour = colourProvider.Background5;
        iconBackground.Colour = colourProvider.Highlight1;
        icon.Colour = colourProvider.Background5;

        BorderColour = ColourInfo.GradientVertical(colourProvider.Background3, colourProvider.Background1);
    }
}
