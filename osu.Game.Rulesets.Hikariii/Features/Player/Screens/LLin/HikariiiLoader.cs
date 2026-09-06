using System;
using System.Threading;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Screens;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics;
using osu.Game.Rulesets.Hikariii.Features.Player.Misc;
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
    private bool canPush { get; set; }

    [BackgroundDependencyLoader]
    private void load(IHikariiiPluginManager pluginManager)
    {
        this.AddInternal(enterExitAnimation);
        AddInternal(loadingSpinner);

        var provider = pluginManager.GetPluginProvider(HikariiiCore.ID);
        if (provider == null) throw new InvalidOperationException($"No provider for core: {HikariiiCore.ID}");

        var config = pluginManager.TryGetPluginConfig<HikariiiCoreConfigManager>(provider);
        if (config == null) throw new InvalidOperationException($"No config for core: {HikariiiCore.ID}");

        config.BindWith(HikariiiCoreSetting.FancyHikariiiLoader, enableEnterLeaveAnimation);
    }

    private readonly CancellationTokenSource cancellation = new();

    protected override void LoadComplete()
    {
        base.LoadComplete();

        TargetScreen = createScreen();
        LoadComponentAsync(TargetScreen, OnScreenLoaded, cancellation.Token);

        enableEnterLeaveAnimation.BindValueChanged(v =>
        {
            if (!v.NewValue)
                canPush = true;
        }, true);
    }

    protected override void Update()
    {
        base.Update();

        if (TargetScreen == null)
            return;

        if (canPush && TargetScreen.LoadState == LoadState.Ready && screenMasked && this.IsCurrentScreen())
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

        this.Delay(600).Schedule(() => canPush = true);
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
        canPush = false;

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
