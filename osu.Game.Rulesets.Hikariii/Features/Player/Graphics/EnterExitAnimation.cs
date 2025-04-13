using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets.Hikariii.Features.Player.Misc;
using osu.Game.Rulesets.Hikariii.Features.Player.Screens.LLin;
using osuTK;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Graphics;

public partial class EnterExitAnimation : InputBlockingContainer
{
    private FillFlowContainer descriptionContainer = null!;
    private Container movingContainer;
    private BeatmapCover beatmapBackground;

    private Box topMovingLine;
    private Box bottomMovingLine;

    private OsuSpriteText titleText;
    private OsuSpriteText nowPlayingText;

    public EnterExitAnimation()
    {
        RelativeSizeAxes = Axes.Both;
        RelativePositionAxes = Axes.Both;
        Y = -1;
    }

    [Resolved]
    private CustomColourProvider colourProvider { get; set; } = null!;

    [Resolved]
    private IBindable<WorkingBeatmap> beatmap { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        Masking = true;

        beatmapBackground = new BeatmapCover(beatmap.Value)
        {
            Size = new Vector2(1.2f),
            Alpha = 0.4f,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            UseBufferedBackground = true
        };

        titleText = new OsuSpriteText
        {
            Text = "???",
            Font = OsuFont.GetFont(size: 64, weight: FontWeight.Black, typeface: Typeface.Venera),
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
        };

        descriptionContainer = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.Both,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(0, 10),

            Children =
            [
                titleText.WithEffect(new GlowEffect
                {
                    Strength = 2f,
                    Colour = Color4Extensions.FromHex("#ABCDEF")
                }),
                nowPlayingText = new OsuSpriteText
                {
                    Text = "???",
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Font = OsuFont.GetFont(size: 24)
                }
            ]
        };

        Box bgBox;
        Children =
        [
            bgBox = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = colourProvider.Background5,
            },
            movingContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Name = "Moving Container",
                Children =
                [
                    beatmapBackground.WithEffect(new BlurEffect
                    {
                        Sigma = new Vector2(10)
                    })
                ]
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Name = "Resizing Container",
                Children =
                [
                    movingContainer.CreateProxy(),
                    descriptionContainer,
                    topMovingLine = new Box
                    {
                        Name = "Top Moving Line",
                        RelativeSizeAxes = Axes.Both,
                        Colour = colourProvider.Highlight1,
                        Anchor = Anchor.TopLeft,
                        Origin = Anchor.TopLeft,
                    },
                    bottomMovingLine = new Box
                    {
                        Name = "Bottom Moving Line",
                        RelativeSizeAxes = Axes.Both,
                        Colour = colourProvider.Highlight1,
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                    }
                ]
            }
        ];

        colourProvider.HueColour.BindValueChanged(_ =>
        {
            topMovingLine.Colour = bottomMovingLine.Colour = colourProvider.Highlight1;
            bgBox.Colour = colourProvider.Background5;
        }, false);

        beatmap.BindValueChanged(v => beatmapBackground.UpdateBackground(v.NewValue));
    }

    private const float move_duration = 500f;
    private const float wait_duration = 600f;
    private const float total_duration = move_duration * 2 + wait_duration;
    private const float line_animation_start_time = 200f;

    public void PlayHide(LocalisableString text, Action onMasked)
    {
        this.Show();

        this.Y = -1;

        titleText.Text = text;
        movingContainer.Y = 0;

        nowPlayingText.Text = beatmap.Value.Metadata.GetDisplayTitleRomanisable();

        movingContainer.MoveToY(30, 300 + 500 + 500);

        bottomMovingLine.FadeIn()
                        .ResizeHeightTo(1)
                        .Then()
                        .Delay(line_animation_start_time)
                        .ResizeHeightTo(0, move_duration, Easing.OutQuint);

        topMovingLine.ResizeHeightTo(0)
                     .Delay(move_duration + wait_duration)
                     .Then()
                     .ResizeHeightTo(1f, move_duration, Easing.OutQuint);

        this.MoveToY(0, move_duration, Easing.OutQuint)
            .Then()
            .Delay(wait_duration)
            .Schedule(onMasked)
            .MoveToY(1, move_duration, Easing.InQuint)
            .Then()
            .FadeOut();
    }

    public void PlayShow(LocalisableString text, Action onMasked)
    {
        this.Y = 1;

        this.Show();

        titleText.Text = text;
        movingContainer.Y = 0;

        nowPlayingText.Text = beatmap.Value.Metadata.GetDisplayTitleRomanisable();

        bottomMovingLine.FadeIn()
                        .ResizeHeightTo(0)
                        .Then()
                        .Delay(move_duration + wait_duration)
                        .ResizeHeightTo(1, 500, Easing.OutQuint);

        topMovingLine.ResizeHeightTo(1)
                     .Delay(line_animation_start_time)
                     .Then()
                     .ResizeHeightTo(0.0f, 500, Easing.OutQuint);

        movingContainer.MoveToY(-30, total_duration);

        this.MoveToY(0, move_duration, Easing.OutQuint)
            .Then()
            .Delay(wait_duration)
            .Schedule(onMasked)
            .MoveToY(-1, move_duration, Easing.InQuint)
            .Then()
            .FadeOut();
    }
}
