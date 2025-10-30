#nullable disable

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Overlays;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Screens.LLin;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SideBar.PluginsPage
{
    public partial class PluginPiece : CompositeDrawable, IHasTooltip
    {
        public readonly LLinPlugin Plugin;
        private RoundedButton disableButton;
        private RoundedButton enableButton;

        [Resolved]
        private CustomColourProvider colourProvider { get; set; }

        public PluginPiece(LLinPlugin pl)
        {
            Plugin = pl;
        }

        [Resolved]
        private IDialogOverlay dialog { get; set; }

        private readonly BindableBool disabled = new BindableBool();
        private GridContainer buttonsGrid;
        private Circle statusCircle;
        private Box bgBox;
        private DelayedLoadUnloadWrapper textureWrapper;

        [Resolved]
        private SessionPluginManager manager { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            Width = 610;
            AutoSizeAxes = Axes.Y;
            AutoSizeDuration = 200;
            AutoSizeEasing = Easing.OutQuint;

            Masking = true;
            CornerRadius = 10;

            BorderColour = Color4.White;

            Anchor = Origin = Anchor.TopCentre;

            var desc = Plugin.Provider.GetDescription();

            InternalChildren = new Drawable[]
            {
                bgBox = new Box
                {
                    RelativeSizeAxes = Axes.Both
                },
                textureWrapper = new DelayedLoadUnloadWrapper(() =>
                {
                    string coverName = Plugin.GetType().Namespace?.Replace(".", "") ?? "Plugin";
                    var s = new PluginBackgroundSprite($"{coverName}/{Plugin.GetType().Name}")
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Alpha = 0
                    };

                    s.OnLoadComplete += d => d.FadeIn(300);

                    return s;
                }, 0)
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.TopRight
                },
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(10),
                    Padding = new MarginPadding(10),
                    Children = new Drawable[]
                    {
                        new FillFlowContainer
                        {
                            Name = "信息FillFlow",
                            AutoSizeAxes = Axes.Y,
                            RelativeSizeAxes = Axes.X,
                            Direction = FillDirection.Vertical,
                            Children = new Drawable[]
                            {
                                new FillFlowContainer
                                {
                                    AutoSizeAxes = Axes.Y,
                                    RelativeSizeAxes = Axes.X,
                                    Spacing = new Vector2(5),

                                    Children =
                                    [
                                        statusCircle = new Circle
                                        {
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.CentreLeft,
                                            Size = new Vector2(7),
                                            Colour = colourProvider.Background5
                                        },
                                        new OsuSpriteText
                                        {
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.CentreLeft,
                                            Text = desc.Name,
                                            Font = OsuFont.GetFont(size: 19)
                                        }
                                    ]
                                },
                                new TruncatingSpriteText
                                {
                                    Text = desc.Description,
                                    Anchor = Anchor.TopLeft,
                                    Origin = Anchor.TopLeft,
                                    Font = OsuFont.GetFont(size: 19)
                                },
                                new TruncatingSpriteText
                                {
                                    Text = desc.AuthorString(),
                                    Anchor = Anchor.TopLeft,
                                    Origin = Anchor.TopLeft,
                                    Font = OsuFont.GetFont(size: 19)
                                }
                            }
                        },
                        buttonsGrid = new GridContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            Height = 30,
                            Alpha = 0,
                            Content = new[]
                            {
                                new Drawable[]
                                {
                                    disableButton = new RoundedButton
                                    {
                                        Height = 30,
                                        RelativeSizeAxes = Axes.X,
                                        Width = 0.95f,
                                        Text = "禁用此插件",
                                        Action = () => manager.DisablePlugin(Plugin),
                                        Enabled = { Value = false },
                                        Anchor = Anchor.BottomCentre,
                                        Origin = Anchor.BottomCentre,
                                    },
                                    enableButton = new RoundedButton
                                    {
                                        Height = 30,
                                        RelativeSizeAxes = Axes.X,
                                        Width = 0.95f,
                                        Text = "启用此插件",
                                        Action = () => manager.EnablePlugin(Plugin),
                                        Enabled = { Value = false },
                                        Anchor = Anchor.BottomCentre,
                                        Origin = Anchor.BottomCentre,
                                    }
                                }
                            }
                        }
                    }
                },
                new HoverClickSounds()
            };

            if (Plugin.Flags.Contains(LLinPlugin.PluginFlags.CanDisable))
            {
                disabled.BindTo(Plugin.Disabled);

                disabled.BindValueChanged(v =>
                {
                    enableButton.Enabled.Value = v.NewValue;
                    disableButton.Enabled.Value = !v.NewValue;
                    statusCircle.FadeColour(v.NewValue
                        ? colourProvider.Background5
                        : colourProvider.Light2, 200, Easing.OutQuint);
                    TooltipText = string.Empty;
                }, true);
            }
            else
            {
                TooltipText = "目前不能通过此面板禁用该插件";
            }

            colourProvider.HueColour.BindValueChanged(_ => updateColors(), true);
        }

        private void updateColors()
        {
            bgBox.Colour = colourProvider.InActiveColor;
            textureWrapper.Colour = ColourInfo.GradientHorizontal(
                Color4.White.Opacity(0.25f),
                Color4.White);

            BorderColour = HasFocus ? colourProvider.Light2 : Color4.White;

            statusCircle.Colour = (Plugin.Disabled.Value ? colourProvider.Background5 : colourProvider.Light2);
        }

        public override void Hide()
        {
            this.FadeOut(200, Easing.OutExpo)
                .MoveToX(-15, 200, Easing.OutExpo)
                .ScaleTo(0.8f, 200, Easing.OutExpo);

            this.Delay(200).Expire();
        }

        public LocalisableString TooltipText { get; set; }

        public override bool AcceptsFocus => true;

        protected override void OnFocus(FocusEvent e)
        {
            BorderColour = colourProvider.Light2;
            buttonsGrid.FadeIn(200, Easing.OutQuint);
            base.OnFocus(e);
        }

        protected override void OnFocusLost(FocusLostEvent e)
        {
            BorderColour = Color4.White;
            BorderThickness = 0f;
            buttonsGrid.FadeOut(200, Easing.OutQuint);
            base.OnFocusLost(e);
        }

        protected override bool OnHover(HoverEvent e)
        {
            if (!HasFocus)
                BorderThickness = 1.5f;

            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            if (!HasFocus)
                BorderThickness = 0f;

            base.OnHoverLost(e);
        }

        protected override bool OnClick(ClickEvent e)
        {
            BorderThickness = 3f;

            if (HasFocus)
            {
                if (Plugin.Disabled.Value)
                    enableButton.TriggerClick();
                else
                    disableButton.TriggerClick();
            }

            return true;
        }

        private partial class PluginBackgroundSprite : Sprite
        {
            private readonly string target;

            public PluginBackgroundSprite(string target = null)
            {
                RelativeSizeAxes = Axes.Both;
                FillMode = FillMode.Fill;

                this.target = target;
            }

            [BackgroundDependencyLoader]
            private void load(LargeTextureStore textures)
            {
                Texture = textures.Get(target);
            }
        }
    }
}
