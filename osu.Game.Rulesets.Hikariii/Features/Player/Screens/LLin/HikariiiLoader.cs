using System;
using System.Collections;
using System.Threading;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osu.Game.Overlays;
using osu.Game.Overlays.Dialog;
using osu.Game.Overlays.Notifications;
using osu.Game.Rulesets.Hikariii.Features.Configuration;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics;
using osu.Game.Rulesets.Hikariii.Features.Player.Misc;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Extensions;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.BuiltIn.Core;
using osu.Game.Screens;
using osuTK;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Screens.LLin;

public partial class HikariiiLoader : OsuScreen
{
    public override bool HideOverlaysOnEnter => true;
    public override bool? ApplyModTrackAdjustments => true;

    private readonly Func<LLinScreen> createScreen;

    public HikariiiLoader(Func<LLinScreen> createScreen)
    {
        this.createScreen = createScreen;
        this.Depth = -100;

        this.enterExitAnimation = new HikariiiLoadAnimation();
        this.loadingSpinner = new LoadingIndicator
        {
            Anchor = Anchor.BottomCentre,
            Origin = Anchor.BottomCentre,
            RelativePositionAxes = Axes.Both,
            Margin = new MarginPadding { Bottom = 125 },
            Size = new Vector2(100),
            Alpha = 0
        };
    }

    protected LLinScreen? TargetScreen;
    private readonly HikariiiLoadAnimation enterExitAnimation;
    private readonly BindableBool enableEnterLeaveAnimation = new(true);
    private readonly LoadingIndicator loadingSpinner;

    private bool screenMasked { get; set; }

    private Stack pushBlockingStack = new();

    private DependencyContainer dependencies;

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    [BackgroundDependencyLoader]
    private void load(IHikariiiPluginManager pluginManager)
    {
        var provider = pluginManager.GetPluginProviderOrThrow(HikariiiCore.ID);
        var config = pluginManager.TryGetPluginConfigOrThrow<HikariiiCoreConfigManager>(provider);

        config.BindWith(HikariiiCoreSetting.EnableFancyIntroOutro, enableEnterLeaveAnimation);
        dependencies.Cache(config);

        this.AddInternal(enterExitAnimation);
        AddInternal(loadingSpinner);
    }

    private readonly CancellationTokenSource cancellation = new();

    [Resolved]
    private Storage storage { get; set; }

    [Resolved]
    private IDialogOverlay dialogOverlay { get; set; }

    [Resolved]
    private INotificationOverlay notificationOverlay { get; set; }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        //region migrate config

        if (storage.Exists("mf.ini"))
        {
            pushBlockingStack.Push(null); // migrate config

            var migrator = new LegacyConfigMigrator();
            this.LoadComponent(migrator);

            var dialog = new ConfirmDialog("检测到旧的播放器设置",
                () => // confirm
                {
                    migrator.StartMigrate();
                    migrator.MoveLegacyConfigFile();

                    notificationOverlay.Post(new SimpleNotification
                    {
                        Text = "迁移已完成，旧的配置将移动到 mf.ini.bak",
                        Icon = FontAwesome.Regular.CheckCircle
                    });

                    pushBlockingStack.Pop();
                },
                () => // cancel
                {
                    migrator.MoveLegacyConfigFile();

                    notificationOverlay.Post(new SimpleNotification
                    {
                        Text = "迁移未进行，旧的配置将移动到 mf.ini.bak"
                    });

                    pushBlockingStack.Pop();
                })
            {
                BodyText = "该版本的 Hikariii 已不再读取 mf.ini\n是否执行配置迁移?"
            };
            dialogOverlay.Push(dialog);
        }

        //endregion migrate config

        TargetScreen = createScreen();
        LoadComponentAsync(TargetScreen, OnScreenLoaded, cancellation.Token);

        /*enableEnterLeaveAnimation.BindValueChanged(v =>
        {
            if (!v.NewValue)
                pushBlockingStack.Pop();
        }, true);*/
    }

    protected override void Update()
    {
        base.Update();

        if (TargetScreen == null)
            return;

        if (pushBlockingStack.Count == 0 && TargetScreen.LoadState == LoadState.Ready && screenMasked && this.IsCurrentScreen())
        {
            if (enableEnterLeaveAnimation.Value)
                enterExitAnimation.FoldToTop();

            ChangeInternalChildDepth(enterExitAnimation, -100);
            screenMasked = false;

            loadingSpinner.Hide();
            this.Push(TargetScreen);
        }
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        if (enableEnterLeaveAnimation.Value)
            enterExitAnimation.AppearFromBottom("HIKARIII PLAYER", () => screenMasked = true);
        else
            screenMasked = true;

        pushBlockingStack.Push(null); // animation
        this.Delay(600).Schedule(() => pushBlockingStack.Pop());
        this.Delay(2500).Schedule(() =>
        {
            if (TargetScreen != null && TargetScreen.LoadState < LoadState.Ready)
                loadingSpinner.Show();
        });

        base.OnEntering(e);
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        // Delay so that our animation doesn't get skipped
        this.FadeTo(0.998f, 1000);

        base.OnSuspending(e);
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        if (!alreadyPlayingExit)
        {
            if (enableEnterLeaveAnimation.Value)
                enterExitAnimation.PlayHide("Leaving Hikariii", exitIfCurrent);
            else
                Schedule(exitIfCurrent);
        }

        alreadyPlayingExit = true;

        base.OnResuming(e);
    }

    private void exitIfCurrent()
    {
        if (this.IsCurrentScreen())
            this.Exit();
    }

    private bool alreadyPlayingExit;

    public override bool OnExiting(ScreenExitEvent e)
    {
        TargetScreen = null;
        cancellation.Cancel();
        pushBlockingStack.Push(this);

        if (!alreadyPlayingExit && enableEnterLeaveAnimation.Value)
            enterExitAnimation.PlayHide("Leaving Hikariii", () => { });

        // Delay so that our animation doesn't get skipped
        this.FadeTo(0.999f, 1000);

        return base.OnExiting(e);
    }

    protected void OnScreenLoaded(LLinScreen screen)
    {
    }
}
