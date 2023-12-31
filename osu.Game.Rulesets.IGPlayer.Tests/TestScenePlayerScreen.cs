using M.Resources;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osu.Game.Graphics.UserInterface;
using osu.Game.Input;
using osu.Game.Overlays;
using osu.Game.Rulesets.IGPlayer.Helper.Configuration;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Screens.LLin;
using osu.Game.Screens;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.IGPlayer.Tests;

public partial class TestScenePlayerScreen : OsuTestScene
{
    private OsuScreenStack stack = null!;

    private BackButton backButton = null!;

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

        Dependencies.Cache(new MConfigManager(storage));
        cacheAndAdd(new LLinPluginManager());

        var dialog = new DialogOverlay
        {
        };

        cacheAndAdd(dialog);
        Dependencies.CacheAs(typeof(IDialogOverlay), dialog);

        // Add Resource store
        gameBase.Resources.AddStore(new DllResourceStore(typeof(IGPlayerRuleset).Assembly));
        gameBase.Resources.AddStore(new DllResourceStore(MResources.ResourceAssembly));

        var notifications = new NotificationOverlay
        {
            RelativeSizeAxes = Axes.Both
        };
        cacheAndAdd(notifications);
        Dependencies.CacheAs(typeof(INotificationOverlay), notifications);

        cacheAndAdd(new IdleTracker(6000));

        var receptor = new BackButton.Receptor();
        cacheAndAdd(receptor);
        cacheAndAdd(backButton = new BackButton(receptor)
        {
            Anchor = Anchor.BottomLeft,
            Origin = Anchor.BottomLeft
        });

        backButton.Action = () =>
        {
            if (backButton.State.Value == Visibility.Visible && stack.CurrentScreen != null) stack.Exit();
        };

        //AddGame(gameInstance = new OsuGame());
        AddStep("Push player", pushPlayer);
    }

    private void screenSwitch(IScreen lastscreen, IScreen newscreen)
    {
        if (newscreen is OsuScreen osuScreen && osuScreen.AllowBackButton)
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

        stack.Push(new LLinScreen());
    }
}
