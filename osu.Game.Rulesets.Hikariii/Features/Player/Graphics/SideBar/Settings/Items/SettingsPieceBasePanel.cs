#nullable disable

using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets.Hikariii.Features.Player.Screens.LLin;
using osuTK;
using osuTK.Graphics;
using osuTK.Input;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SideBar.Settings.Items
{
    public partial class SettingsPieceBasePanel : CompositeDrawable
    {
        protected readonly OsuSpriteText SpriteText = new OsuSpriteText
        {
            Font = OsuFont.GetFont(size: 20)
        };

        protected readonly SpriteIcon SpriteIcon = new SpriteIcon
        {
            Size = new Vector2(25)
        };

        protected GridContainer FirstLineGrid { get; private set; }

        public virtual LocalisableString Description
        {
            get => description;
            set
            {
                description = value;
                SpriteText.Text = value;
            }
        }

        public virtual IconUsage Icon
        {
            get => icon;
            set
            {
                icon = value;
                SpriteIcon.Icon = value;
                haveIconSet = true;
            }
        }

        protected virtual Drawable CreateSideDrawable() => new PlaceHolder();
        protected virtual IconUsage DefaultIcon => FontAwesome.Regular.QuestionCircle;

        private LocalisableString description;
        private IconUsage icon;
        protected Box BgBox;

        [Resolved]
        private CustomColourProvider colourProvider { get; set; }

        protected CustomColourProvider ColourProvider => colourProvider;

        private bool haveIconSet;
        private Box flashBox;
        protected FillFlowContainer FillFlow;

        private Sample sampleOnClick;

        public static readonly float SinglePanelWidth = 180f;

        public SettingsPieceBasePanel()
        {
            Masking = true;
            CornerRadius = 7.5f;
            AutoSizeAxes = Axes.Y;
            Width = SinglePanelWidth;
            Anchor = Origin = Anchor.TopRight;
        }

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            InternalChildren = new Drawable[]
            {
                BgBox = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Depth = float.MaxValue
                },
                FillFlow = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Y,
                    RelativeSizeAxes = Axes.X,
                    Direction = FillDirection.Vertical,
                    Padding = new MarginPadding(10),
                    Spacing = new Vector2(5),
                    Children = new Drawable[]
                    {
                        FirstLineGrid = new GridContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            RowDimensions = [new Dimension(GridSizeMode.AutoSize)],
                            ColumnDimensions =
                            [
                                new Dimension(GridSizeMode.AutoSize),
                                new Dimension(),
                            ],
                            Content = new[]
                            {
                                new Drawable[]
                                {
                                    SpriteIcon,
                                    CreateSideDrawable().With(d =>
                                    {
                                        d.Anchor = Anchor.CentreLeft;
                                        d.Origin = Anchor.CentreLeft;
                                        d.Margin = new MarginPadding
                                        {
                                            Horizontal = 5,
                                        };
                                    }),
                                },
                            },
                        },
                        SpriteText,
                    }
                },
                flashBox = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.White,
                    Alpha = 0,
                    Depth = -1
                },
                new HoverSounds()
            };

            colourProvider.HueColour.BindValueChanged(_ => OnColorChanged(), true);

            if (!haveIconSet) SpriteIcon.Icon = DefaultIcon;

            sampleOnClick = audio.Samples.Get("UI/default-select");
        }

        protected virtual void OnColorChanged()
        {
            BgBox.Colour = colourProvider.InActiveColor;
            FillFlow.Colour = Color4.White;
        }

        protected virtual void OnLeftClick() { }
        protected virtual void OnRightClick() { }
        protected virtual void OnMiddleClick() { }

        protected override bool OnHover(HoverEvent e)
        {
            flashBox.FadeTo(0.1f, 300);
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            base.OnHoverLost(e);
            flashBox.FadeTo(0f, 300);
        }

        private MouseButton mouseDownButton;

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            mouseDownButton = e.Button;

            return base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            if (e.Button == mouseDownButton && IsHovered)
            {
                sampleOnClick?.Play();

                switch (e.Button)
                {
                    case MouseButton.Left:
                        OnLeftClick();
                        break;

                    case MouseButton.Right:
                        OnRightClick();
                        break;

                    case MouseButton.Middle:
                        OnMiddleClick();
                        break;
                }
            }

            base.OnMouseUp(e);
        }
    }
}
