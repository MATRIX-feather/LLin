using System;
using System.Reflection;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets.Hikariii.Features.Player.Screens.LLin;
using osu.Game.Rulesets.Hikariii.Graphics;
using osu.Game.Screens.Menu;
using osu.Game.Screens.Select;
using osuTK;
using osuTK.Graphics;
using IconButton = osu.Game.Rulesets.Hikariii.Graphics.IconButton;

namespace osu.Game.Rulesets.Hikariii.Features.ListenerLoader.Handlers;

public partial class HikariiiFeatureBoxListener : AbstractHandler
{
    [Resolved(canBeNull: true)]
    private IBindable<RulesetInfo>? ruleset { get; set; }

    [Resolved]
    private Bindable<WorkingBeatmap> beatmap { get; set; } = null!;

    [Resolved]
    private CustomColourProvider colourProvider { get; set; } = null!;

    private BasicDropdownContainer? dropdownContainer;

    private Container contentContainer = null!;
    private OsuSpriteText timeDisplay = null!;
    private OsuSpriteText weekDisplay = null!;

    private Box? background;
    private ClickableContainer clickToCloseTrigger = null!;

    protected override void Update()
    {
        timeDisplay.Text = $"{DateTimeOffset.Now:hh\\:mm}";
        weekDisplay.Text = $"{DateTimeOffset.Now:dddd}";

        base.Update();
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        if (ruleset is not Bindable<RulesetInfo> rs) return;

        MethodInfo? mthRegisterBlockingOverlay = Game.GetType().GetMethod("osu.Game.Overlays.IOverlayManager.RegisterBlockingOverlay",
            BindingFlags.NonPublic | BindingFlags.Instance);

        if (mthRegisterBlockingOverlay == null)
        {
            Logging.Log("Can't register blocking overlay. DO NOT REPORT THIS ISSUE TO THE OFFICIAL OSU! FORUM, NOR THEIR GITHUB!!!!!!", level: LogLevel.Important);
            Logging.Log("Can't register blocking overlay. DO NOT REPORT THIS ISSUE TO THE OFFICIAL OSU! FORUM, NOR THEIR GITHUB!!!!!!");
            Logging.Log("Can't register blocking overlay. DO NOT REPORT THIS ISSUE TO THE OFFICIAL OSU! FORUM, NOR THEIR GITHUB!!!!!!");
            Logging.Log("Can't register blocking overlay. DO NOT REPORT THIS ISSUE TO THE OFFICIAL OSU! FORUM, NOR THEIR GITHUB!!!!!!");
            Logging.Log("Can't register blocking overlay. DO NOT REPORT THIS ISSUE TO THE OFFICIAL OSU! FORUM, NOR THEIR GITHUB!!!!!!");
            Logging.Log("Can't register blocking overlay. DO NOT REPORT THIS ISSUE TO THE OFFICIAL OSU! FORUM, NOR THEIR GITHUB!!!!!!");
            return;
        }

        OsuFocusedOverlayContainer aboutHikariiiOverlay = new AboutHikariiiOverlay
        {
            RelativeSizeAxes = Axes.Both,
            Alpha = 0,

            FadeOutWaitDuration = 301,
            OnFadeOut = overlayFadeOut,
            OnFadeIn = overlayFadeIn,

            Children =
            [
                clickToCloseTrigger = new ClickableContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = background = new Box
                    {
                        RelativeSizeAxes = Axes.Both
                    }
                },
                contentContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding(24),
                    Depth = -1
                }
            ]
        };

        clickToCloseTrigger.Action = aboutHikariiiOverlay.Hide;

        mthRegisterBlockingOverlay.Invoke(Game, [aboutHikariiiOverlay]);

        LinkFlowContainer links;
        contentContainer.AddRange(
        [
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,

                Name = "Clock Container",

                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,

                Spacing = new Vector2(10),

                Children =
                [
                    timeDisplay = new OsuSpriteText
                    {
                        Name = "Time display",
                        Text = "17:35",
                        Font = OsuFont.GetFont(typeface: Typeface.TorusAlternate, size: 72),
                        UseFullGlyphHeight = false,

                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                    },
                    weekDisplay = new OsuSpriteText
                    {
                        Name = "Week display",
                        Text = "周三",
                        Font = OsuFont.GetFont(typeface: Typeface.TorusAlternate, size: 32),
                        UseFullGlyphHeight = false,

                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                    }
                ]
            },
            new Container
            {
                AutoSizeAxes = Axes.Both,
                CornerRadius = 15,
                Masking = true,

                Anchor = Anchor.BottomRight,
                Origin = Anchor.BottomRight,

                Children =
                [
                    new Box
                    {
                        Colour = Color4.Red.Opacity(0.5f),
                        RelativeSizeAxes = Axes.Both
                    },
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Margin = new MarginPadding(10),
                        Spacing = new Vector2(10),
                        Direction = FillDirection.Vertical,

                        Children =
                        [
                            new OsuSpriteText
                            {
                                Text = "!!! WARNING !!!",
                                Font = OsuFont.GetFont(size: 30, weight: FontWeight.Bold, typeface: Typeface.Torus),
                            },
                            links = new LinkFlowContainer(spriteText =>
                            {
                                spriteText.Font = OsuFont.GetFont(size: 20, weight: FontWeight.Bold, typeface: Typeface.Torus);
                            })
                            {
                                MaximumSize = new Vector2(400),
                                AutoSizeAxes = Axes.Both
                            }
                        ]
                    }
                ]
            },
            new OsuAnimatedButton
            {
                Size = new Vector2(18),
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                //Margin = new MarginPadding(24), //24 - 24 -> We don't need it anymore.
                Action = aboutHikariiiOverlay.Hide,
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
        ]);

        links.AddText("连接到 osu! 官方服务器时使用诸如 Hikariii 这类 ruleset 插件可能会导致你的账号被 [[ 封禁 ]]");
        links.AddText("\n");
        links.AddText("欲了解详情请参阅：");
        links.AddText("\n");
        links.AddLink("Cai1Hsu/osu-plugins#93", "https://github.com/Cai1Hsu/osu-plugins/issues/93");
        links.AddText(" 以及 ");
        links.AddLink("ppy/osu#37540", "https://github.com/ppy/osu/issues/37540");

        // Load dropdown menu
        LoadComponentAsync(createFeatureMenu(), loaded =>
        {
            loaded.State.BindValueChanged(v =>
            {
                aboutHikariiiOverlay.FadeTo(v.NewValue == Visibility.Visible ? 1f : 0f, 300, Easing.OutQuint);
            });

            dropdownContainer = loaded;
            aboutHikariiiOverlay.Add(loaded);
        });

        //用户切换到此Ruleset时转到播放器界面
        ruleset.BindValueChanged(v =>
        {
            if (v.NewValue.ShortName != HikariiiPlayerRuleset.SHORT_NAME // 没有切换到我们的Ruleset
                || v.OldValue == null // 开局就设置到了Hikariii
                || v.OldValue.ShortName == HikariiiPlayerRuleset.SHORT_NAME) // 疑似有人TriggerChange (maybe?)
            {
                return;
            }

            rs.Value = v.OldValue;

            aboutHikariiiOverlay.Show();
        });
    }

    private void overlayFadeOut()
    {
        dropdownContainer?.Hide();
        clickToCloseTrigger.FadeOut(300);
        contentContainer.FadeOut(300);
    }

    private void overlayFadeIn()
    {
        dropdownContainer?.Show();
        clickToCloseTrigger.FadeIn(300);
        contentContainer.FadeIn(300);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        if (background == null) return;

        colourProvider.HueColour.BindValueChanged(_ =>
        {
            background.Colour = colourProvider.Background6.Opacity(0.8f);
        }, true);
    }

    private BasicDropdownContainer createFeatureMenu()
    {
        var overlay = new AboutHikariiiDropdownContainer();

        overlay.ButtonContainer.AddRange(
        [
            new IconButton
            {
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,

                Size = new Vector2(150, 30),
                Icon = FontAwesome.Solid.Play,
                Text = "打开 Hikariii 播放器",
                Action = () =>
                {
                    Game.PerformFromScreen(s => s.Push(new HikariiiLoader(() => new LLinScreen())), new[]
                    {
                        typeof(MainMenu),
                        typeof(SoloSongSelect)
                    });
                }
            },
            new IconButton
            {
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,

                Size = new Vector2(75, 30),
                Icon = FontAwesome.Solid.Home,
                Text = "项目主页",
                Action = () =>
                {
                    Game.HandleLink("https://github.com/MATRIX-feather/LLin");
                }
            }
        ]);

        return overlay;
    }
}
