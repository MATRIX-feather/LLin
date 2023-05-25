using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Logging;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Overlays.Toolbar;
using osu.Game.Rulesets.IGPlayer.Player.DownloadAccel;
using osu.Game.Rulesets.IGPlayer.Player.Extensions;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.IGPlayer.Player.Graphics;

public partial class AccelOptionContainer : Container
{
    public AccelOptionContainer(APIBeatmapSet apiSet)
    {
        this.apiBeatmapSet = apiSet;
    }

    private readonly APIBeatmapSet apiBeatmapSet;

    private AccelBeatmapDownloadTracker tracker;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;
        Masking = true;
        CornerRadius = 4;

        this.Y = Toolbar.HEIGHT + 8;

        this.Alpha = 0.001f;

        if (apiBeatmapSet == null)
        {
            this.Expire();
            return;
        }

        tracker = new AccelBeatmapDownloadTracker(apiBeatmapSet)
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        };

        OsuAnimatedButton closeButton;

        InternalChildren = new Drawable[]
        {
            tracker,
            new Box
            {
                Colour = Color4.Black.Opacity(0.6f),
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
                Spacing = new Vector2(7.5f),
                Margin = new MarginPadding(24),
                Direction = FillDirection.Vertical,
                Children = new Drawable[]
                {
                    new OsuSpriteText
                    {
                        Text = apiBeatmapSet.GetDisplayTitle(),
                        Font = OsuFont.GetFont(size: 18),
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Margin = new MarginPadding { Horizontal = 16 }
                    },
                    new FillFlowContainer
                    {
                        Height = 40,
                        AutoSizeAxes = Axes.X,
                        Spacing = new Vector2(5),
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Children = new Drawable[]
                        {
                            new AccelDownloadButton(apiBeatmapSet),
                            new AccelDownloadButton(apiBeatmapSet, true)
                        }
                    }
                }
            }
        };

        closeButton.Colour = Color4.White;
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
}
