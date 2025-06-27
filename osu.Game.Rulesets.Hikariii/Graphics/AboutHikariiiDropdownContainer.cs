using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.API;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Users.Drawables;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hikariii.Graphics;

public partial class AboutHikariiiDropdownContainer : BasicDropdownContainer
{
    protected override bool StartHidden { get; } = true;

    public AboutHikariiiDropdownContainer()
    {
        ButtonContainer = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.Both,
            Padding = new MarginPadding(24),
            Direction = FillDirection.Full,

            Anchor = Anchor.BottomLeft,
            Origin = Anchor.BottomLeft,
        };
    }

    protected override void OnColorUpdated()
    {
        base.OnColorUpdated();

        gradientFillFlow.Colour = ColourProvider.Background5;
        backgroundMask.Colour = ColourProvider.Background5.Opacity(0.9f);
    }

    private partial class HoverBox : Box
    {
        protected override bool OnHover(HoverEvent e)
        {
            this.FadeTo(0.6f, 500, Easing.OutQuint);
            return false;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            this.FadeTo(1, 500, Easing.OutQuint);
            base.OnHoverLost(e);
        }
    }

    private LoopingContainer loopingContainer;
    private Box backgroundMask;
    private Box dimBox;
    private FillFlowContainer gradientFillFlow;

    [BackgroundDependencyLoader]
    private void load()
    {
        BackgroundContainer.Children =
        [
            loopingContainer = new LoopingContainer
            {
                Name = "Avatar/Beatmap background flow",
                RelativeSizeAxes = Axes.Y,
                Width = 1300,
                Children = createBackgroundFlow(),
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Masking = true
            },
            gradientFillFlow = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,

                Children =
                [
                    dimBox = new Box
                    {
                        RelativeSizeAxes = Axes.Y,
                        Width = 1200,
                    },
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Width = 0.1f,
                        Colour = ColourInfo.GradientHorizontal(Color4.White, Color4.White.Opacity(0.666f)),
                    },
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Width = 0.1f,
                        Colour = ColourInfo.GradientHorizontal(Color4.White.Opacity(0.666f), Color4.White.Opacity(0.333f)),
                    },
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Width = 0.1f,
                        Colour = ColourInfo.GradientHorizontal(Color4.White.Opacity(0.333f), Color4.White.Opacity(0f)),
                    }
                ]
            },
            backgroundMask = new HoverBox
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
            }
        ];

        ContentContainer.AddRange(
        [
            new OsuAnimatedButton
            {
                Size = new Vector2(18),
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Margin = new MarginPadding(24),
                Action = this.Hide,
                Children =
                [
                    new SpriteIcon
                    {
                        Icon = FontAwesome.Solid.Times,
                        RelativeSizeAxes = Axes.Both,
                        Scale = new Vector2(0.8f),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    }
                ]
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(24),
                Direction = FillDirection.Vertical,
                Children =
                [
                    new OsuSpriteText
                    {
                        Text = "Hikariii",
                        Font = OsuFont.GetFont(size: 24)
                    },
                    new OsuSpriteText
                    {
                        Text = "一些从 mfosu 中分离出来的功能"
                    }
                ]
            },
            ButtonContainer
        ]);
    }

    public FillFlowContainer ButtonContainer { get; private set; }

    private List<Drawable> createBackgroundFlow()
    {
        List<APIUser> validUsers =
        [
            // mfosu贡献者
            new() { Id = 13870362, Username = "MATRIX-feather" },
            new() { Id = 5321112, Username = "Jason House" },
            new() { Id = 17268434, Username = "PercyDan" },
            new() { Id = 10235769, Username = "BrokenShine" },
            new() { Id = 13851970, Username = "pedajilao" },

            // 一些认识的人
            new() { Id = 30973609, Username = "MoeRain233" }, // SysZ-Future
            new() { Id = 19890921, Username = "FoxMeow" }, // FoxMeow
            new() { Id = 6763446, Username = "Xiaracto" },
            new() { Id = 13871278, Username = "12727337483" }, // 12727337483
        ];

        int childCount = validUsers.Count;
        List<Drawable> flow = [];

        for (int i = 0; i < childCount; i++)
        {
            Container subChild;
            LoadingSpinner spinner;

            var child = new Container
            {
                RelativeSizeAxes = Axes.Y,
                Width = TargetHeight,

                Child = subChild = new Container
                {
                    RelativeSizeAxes = Axes.Both,

                    //不知为何有视觉瑕疵：如果物件超出屏幕范围则不会被渲染
                    //Shear = new Vector2(0.15f, 0),

                    Children =
                    [
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Colour4.Gray
                        },
                        spinner = new LoadingSpinner(true)
                        {
                            RelativeSizeAxes = Axes.Both,
                            Size = new Vector2(0.3f),
                            Depth = -10
                        }
                    ]
                }
            };

            spinner.Show();

            APIAccess apiAccess;

            this.LoadComponentAsync(new ClickableAvatar(user: getCircular(validUsers, i)), loaded =>
            {
                loaded.Action += this.Hide;
                loaded.RelativeSizeAxes = Axes.Both;
                subChild.FillMode = FillMode.Fill;
                subChild.Add(loaded);

                spinner.Hide();
            });

            flow.Add(child);
        }

        return flow;
    }

    private X getCircular<X>(List<X> flow, int index)
    {
        int maxSize = flow.Count;

        return flow[index % maxSize];
    }

    protected override void UpdateAfterChildren()
    {
        base.UpdateAfterChildren();

        float loopingWidth = Math.Min(1000, this.DrawWidth * 0.6f);
        loopingContainer.Width = loopingWidth;

        dimBox.Width = this.DrawWidth - loopingWidth;
        backgroundMask.Width = loopingWidth / this.DrawWidth;

        ButtonContainer.Width = 0.4f;
    }
}
