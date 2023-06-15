using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Game.Beatmaps.Drawables;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Rulesets.IGPlayer.Player.Extensions;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.IGPlayer.DownloadAccel.Graphics;

public partial class AccelOptionContainer : Container
{
    public AccelOptionContainer(APIBeatmapSet apiSet)
    {
        this.apiBeatmapSet = apiSet;
    }

    private readonly APIBeatmapSet apiBeatmapSet;

    private AccelBeatmapDownloadTracker tracker = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;
        Masking = true;
        CornerRadius = 8;

        this.Alpha = 0.001f;

        this.EdgeEffect = new EdgeEffectParameters
        {
            Type = EdgeEffectType.Shadow,
            Colour = Color4.Black.Opacity(0.3f),
            Radius = 4,
            Offset = new Vector2(0, 2)
        };

        tracker = new AccelBeatmapDownloadTracker(apiBeatmapSet)
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        };

        OsuAnimatedButton closeButton;
        FillFlowContainer buttonFillFlow;
        var onlineCover = new OnlineBeatmapSetCover(apiBeatmapSet, BeatmapSetCoverType.Card)
        {
            RelativeSizeAxes = Axes.Both,
            Colour = Color4Extensions.FromHex("#3f3f3f"),
            FillMode = FillMode.Fill,
            Alpha = 0.001f,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        };

        onlineCover.OnLoadComplete += d =>
        {
            d.FadeIn(300, Easing.OutQuint);
        };

        InternalChildren = new Drawable[]
        {
            tracker,
            new Box
            {
                Colour = Color4.Black.Opacity(0.6f),
                RelativeSizeAxes = Axes.Both
            },
            new DelayedLoadUnloadWrapper(() => onlineCover, 10)
            {
                RelativeSizeAxes = Axes.Both
            },
            closeButton = new OsuAnimatedButton
            {
                Size = new Vector2(18),
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Margin = new MarginPadding(12),
                Action = this.Hide
            },
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Spacing = new Vector2(12.5f),
                Margin = new MarginPadding(24),
                Direction = FillDirection.Vertical,
                Children = new Drawable[]
                {
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Spacing = new Vector2(5f),
                        Direction = FillDirection.Vertical,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Children = new Drawable[]
                        {
                            new OsuSpriteText
                            {
                                Text = apiBeatmapSet.GetDisplayTitle(),
                                Font = OsuFont.GetFont(size: 20, typeface: Typeface.TorusAlternate),
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                Margin = new MarginPadding { Horizontal = 16 }
                            },
                            new OsuSpriteText
                            {
                                Text = apiBeatmapSet.GetDisplayArtist(),
                                Font = OsuFont.GetFont(size: 14, typeface: Typeface.TorusAlternate, weight: FontWeight.SemiBold),
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                Margin = new MarginPadding { Horizontal = 16 }
                            }
                        }
                    },
                    buttonFillFlow = new FillFlowContainer
                    {
                        Height = 40,
                        AutoSizeAxes = Axes.X,
                        Spacing = new Vector2(5),
                        Margin = new MarginPadding { Top = 5 },
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Child = new AccelDownloadButton(apiBeatmapSet)
                    }
                }
            }
        };

        if (apiBeatmapSet.HasVideo)
            buttonFillFlow.Add(new AccelDownloadButton(apiBeatmapSet, true));

        closeButton.Add(new SpriteIcon
        {
            Icon = FontAwesome.Solid.Times,
            RelativeSizeAxes = Axes.Both,
            Scale = new Vector2(0.8f),
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        });

        tracker.State.BindValueChanged(this.OnStateChanged, true);
    }

    [Resolved]
    private OsuGame game { get; set; } = null!;

    protected override void UpdateAfterChildren()
    {
        base.UpdateAfterChildren();
        var toolbar = game.Toolbar;
        this.Y = toolbar.Y + toolbar.Height + 8;
    }

    public override void Hide()
    {
        this.FadeOut(300, Easing.OutQuint)
            .MoveToX(-8, 300, Easing.OutQuint);
    }

    public override void Show()
    {
        this.FadeIn(300, Easing.OutQuint)
            .MoveToX(8, 300, Easing.OutQuint);
    }

    private void OnStateChanged(ValueChangedEvent<DownloadState> v)
    {
        Logger.Log($"State changed! {v.OldValue} -> {v.NewValue}");

        switch (v.NewValue)
        {
            case DownloadState.LocallyAvailable:
                this.Hide();
                this.Expire();
                break;

            default:
                this.Show();
                break;
        }
    }

    protected override bool OnClick(ClickEvent e)
    {
        return true;
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        return true;
    }
}
