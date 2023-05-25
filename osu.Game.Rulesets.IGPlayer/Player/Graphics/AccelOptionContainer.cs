using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics.Sprites;
using osu.Game.Online;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Rulesets.IGPlayer.Player.DownloadAccel;
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

        tracker = new AccelBeatmapDownloadTracker(apiBeatmapSet)
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        };

        InternalChildren = new Drawable[]
        {
            tracker,
            new Box
            {
                Colour = Color4.Black.Opacity(0),
                Size = new Vector2(40, 120),
            },
            new Box
            {
                Colour = Color4.Black.Opacity(0.6f),
                RelativeSizeAxes = Axes.Both
            },
            new OsuSpriteText
            {
                Text = apiBeatmapSet.Title
            },
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.X,
                Height = 40,
                Spacing = new Vector2(5),
                Margin = new MarginPadding(16),
                Children = new[]
                {
                    new AccelDownloadButton(apiBeatmapSet),
                    new AccelDownloadButton(apiBeatmapSet, true)
                }
            }
        };

        tracker.State.BindValueChanged(this.OnStateChanged, true);
    }

    private void OnStateChanged(ValueChangedEvent<DownloadState> v)
    {
        switch (v.NewValue)
        {
            case DownloadState.LocallyAvailable:
                this.FadeOut(200, Easing.OutQuint).Expire();
                AddInternal(new OsuSpriteText
                {
                    Text = "已有此图"
                });
                break;
        }
    }
}
