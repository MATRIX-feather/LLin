using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Screens;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets.Hikariii.Features.Player.Screens.LLin;
using osu.Game.Rulesets.Hikariii.Graphics;
using osu.Game.Screens.Menu;
using osuTK;

namespace osu.Game.Rulesets.Hikariii.Features.ListenerLoader.Handlers;

public partial class HikariiiFeatureBoxListener : AbstractHandler
{
    [Resolved(canBeNull: true)]
    private IBindable<RulesetInfo>? ruleset { get; set; }

    [Resolved]
    private Bindable<WorkingBeatmap> beatmap { get; set; } = null!;

    [Resolved]
    private CustomColourProvider colourProvider { get; set; } = null!;

    private BasicDropdownContainer? featureHeaderOverlay;

    private Container contentContainer;
    private OsuSpriteText timeDisplay;
    private OsuSpriteText weekDisplay;
    private Box background;

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

        Container masterContainer;
        Game.Add(masterContainer = new Container
        {
            Name = "HikariiiFeatureBoxBackground",
            RelativeSizeAxes = Axes.Both,
            Alpha = 0,
            Children =
            [
                background = new Box
                {
                    RelativeSizeAxes = Axes.Both
                },
                contentContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding(24),
                    Children =
                    [
                        new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,

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
                        }
                    ]
                }
            ]
        });

        LoadComponentAsync(createFeatureMenu(), loaded =>
        {
            loaded.State.BindValueChanged(v =>
            {
                masterContainer.FadeTo(v.NewValue == Visibility.Visible ? 1f : 0f, 300, Easing.OutQuint);
            });
            featureHeaderOverlay = loaded;

            Game.Add(loaded);
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

            this.featureHeaderOverlay?.Show();
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        colourProvider.HueColour.BindValueChanged(_ =>
        {
            background.Colour = colourProvider.Background6.Opacity(0.8f);
        }, true);
    }

    private BasicDropdownContainer createFeatureMenu()
    {
        var overlay = new AboutHikariiiDropdownContainer
        {
        };

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
                    Game.PerformFromScreen(s => s.Push(new LLinLoader(() => new LLinScreen())), new[]
                    {
                        typeof(MainMenu)
                    });

                    overlay.Hide();
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
                    overlay.Hide();
                }
            }
        ]);

        return overlay;
    }
}
