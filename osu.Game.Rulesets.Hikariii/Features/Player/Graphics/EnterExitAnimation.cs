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
using osu.Game.Rulesets.Hikariii.Features.Configuration;
using osu.Game.Rulesets.Hikariii.Features.Player.Misc;
using osu.Game.Rulesets.Hikariii.Features.Player.Screens.LLin;
using osuTK;
using osuTK.Graphics;

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

    private readonly BindableBool triangelesUseV2 = new BindableBool();

    [BackgroundDependencyLoader]
    private void load(MConfigManager config)
    {
        Masking = true;

        config.BindWith(MSetting.MvisUseTriangleV2, triangelesUseV2);

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
            new MBgTriangles(trianglesColor: Color4.White, withBeat: true, alpha: 0.9f, triangleScale: 4f)
            {
                RelativeSizeAxes = Axes.Both,
                UseV2 = { BindTarget = triangelesUseV2 }
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
        });

        beatmap.BindValueChanged(v => beatmapBackground.UpdateBackground(v.NewValue));
    }

    public void PlayHide(LocalisableString text, Action onMasked)
    {
        this.Show();

        titleText.Text = text;
        movingContainer.Y = 0;

        nowPlayingText.Text = beatmap.Value.Metadata.GetDisplayTitleRomanisable();

        const float move_duration = 300f;
        const float wait_duration = 0f;
        const float total_duration = move_duration * 2 + wait_duration;
        const float line_animation_start_time = 200f;

        movingContainer.MoveToY(30, total_duration);

        topMovingLine.ResizeHeightTo(1f, move_duration, Easing.OutQuint);

        this.MoveToY(0, move_duration, Easing.OutQuint)
            .Then()
            //.Delay(wait_duration)
            .Schedule(onMasked)
            .MoveToY(1, move_duration / 2f, Easing.OutQuint)
            .Then()
            .FadeOut();
    }

    public void AppearFromBottom(LocalisableString text, Action onMasked)
    {
        this.Y = 1;

        this.Show();

        titleText.Text = text;
        movingContainer.Y = 30;

        nowPlayingText.Text = beatmap.Value.Metadata.GetDisplayTitleRomanisable();

        const float move_duration = 500f;
        const float wait_duration = 600f;
        const float total_duration = move_duration * 2 + wait_duration;
        const float line_animation_start_time = 200f;

        // 底边往上延展扩展的线
        bottomMovingLine.FadeIn()
                        .ResizeHeightTo(0);

        // 顶端往上收回的线
        topMovingLine.ResizeHeightTo(1)
                     .Delay(line_animation_start_time)
                     .Then()
                     .ResizeHeightTo(0.0f, 500, Easing.OutQuint);

        // 谱面背景的动画
        movingContainer.MoveToY(-30, 5000, Easing.InOutSine)
                       .Then()
                       .MoveToY(30, 5000, Easing.InOutSine)
                       .Loop();

        // 整体移动
        this.MoveToY(0, move_duration, Easing.OutQuint)
            .Then()
            .Delay(wait_duration)
            .Schedule(onMasked);
    }

    public void FoldToTop()
    {
        const float move_duration = 500f;
        const float wait_duration = 600f;

        bottomMovingLine.ResizeHeightTo(1, 500, Easing.OutQuint);
        this.MoveToY(-1, move_duration, Easing.InQuint);
    }
}
