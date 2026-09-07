using M.Resources;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osu.Game.Database;
using osu.Game.Graphics.UserInterface;
using osu.Game.Input;
using osu.Game.Overlays;
using osu.Game.Rulesets.Hikariii.Features.Configuration;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Loader;
using osu.Game.Rulesets.Hikariii.Features.Player.Screens.LLin;
using osu.Game.Screens;
using osu.Game.Screens.Footer;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Hikariii.Tests.HikariiiPlayerv2;

public partial class TestSceneSongPlayerScreenBase : OsuTestScene
{
    private OsuScreenStack stack = null!;

    private BackButton backButton = null!;
    private ScreenStackFooter screenFooter = null!;
    protected HikariiiPluginHub PluginHub;

    protected LLinScreen? CurrentLLin { get; set; }

    public TestSceneSongPlayerScreenBase()
    {
        PluginHub = new HikariiiPluginHub();
    }

    [Resolved]
    private HikariiiTestBrowser testBrowser { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load(Storage storage, OsuGameBase gameBase)
    {
        stack = new OsuScreenStack
        {
            RelativeSizeAxes = Axes.Both
        };

        cacheAndAdd(stack);
        stack.ScreenPushed += screenSwitch;
        stack.ScreenExited += screenSwitch;

        Dependencies.Cache(new CustomColourProvider());
        Dependencies.Cache(new LLinGlobalConfigManager(storage));

        cacheAndAdd(PluginHub);
        Dependencies.CacheAs(typeof(IHikariiiPluginManager), PluginHub);
        PluginHub.LoadFrom(new HikariiiBundledPluginLoader(), out _);

        var dialog = new DialogOverlay();

        cacheAndAdd(dialog);
        Dependencies.CacheAs(typeof(IDialogOverlay), dialog);

        // Add Resource store
        gameBase.Resources.AddStore(new DllResourceStore(typeof(HikariiiPlayerRuleset).Assembly));
        gameBase.Resources.AddStore(new DllResourceStore(MResources.ResourceAssembly));

        var notifications = new NotificationOverlay
        {
            RelativeSizeAxes = Axes.Both
        };
        cacheAndAdd(notifications);
        Dependencies.CacheAs(typeof(INotificationOverlay), notifications);

        cacheAndAdd(new IdleTracker(6000));

        cacheAndAdd(backButton = new BackButton
        {
            Anchor = Anchor.BottomLeft,
            Origin = Anchor.BottomLeft,
            Action = () =>
            {
                if (backButton.State.Value == Visibility.Visible && stack.CurrentScreen != null) stack.Exit();
            }
        });

        screenFooter = new ScreenStackFooter(stack, new ScreenFooter.BackReceptor())
        {
            BackButtonPressed = stack.Exit
        };

        Add(new PopoverContainer
        {
            Child = screenFooter,
            RelativeSizeAxes = Axes.Both,
            Depth = -999,
        });

        var beatmapStore = new RealmDetachedBeatmapStore();
        Dependencies.CacheAs(typeof(BeatmapStore), beatmapStore);
        this.Add(beatmapStore);

        //AddGame(gameInstance = new OsuGame());
        AddStep("Push player", pushPlayer);

        AddToggleStep("切换Host光标", v =>
        {
            testBrowser.OsuHostCursorVisible.Value = v;
            testBrowser.SystemCursorVisible.Value = !v;
        });
    }

    private void screenSwitch(IScreen lastscreen, IScreen newscreen)
    {
        if (newscreen is OsuScreen osuScreen && osuScreen.BackButtonVisibility.Value)
            backButton.Show();
        else
            backButton.Hide();
    }

    private void cacheAndAdd(Drawable drawable)
    {
        Dependencies.Cache(drawable);
        Add(drawable);
    }

    private void pushPlayer()
    {
        while (stack.CurrentScreen != null)
            stack.Exit();

        var next = new LLinScreen();
        stack.Push(CurrentLLin = next);
        next.Exiting += () =>
        {
            if (CurrentLLin == next)
                CurrentLLin = null;
        };
    }
}
