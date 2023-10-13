using M.Resources;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Game.Input;
using osu.Game.Overlays;
using osu.Game.Rulesets.IGPlayer.Configuration;
using osu.Game.Rulesets.IGPlayer.Player.Plugins;
using osu.Game.Rulesets.IGPlayer.Player.Screens.LLin;
using osu.Game.Screens;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.IGPlayer.Tests;

public partial class TestScenePlayerScreen : OsuTestScene
{
    private OsuScreenStack stack = null!;

    [BackgroundDependencyLoader]
    private void load(Storage storage, OsuGameBase gameBase)
    {
        stack = new OsuScreenStack
        {
            RelativeSizeAxes = Axes.Both
        };
        cacheAndAdd(stack);

        Dependencies.Cache(new MConfigManager(storage));
        cacheAndAdd(new LLinPluginManager());

        var dialog = new DialogOverlay
        {
            RelativeSizeAxes = Axes.Both
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

        //AddGame(gameInstance = new OsuGame());
        AddStep("Push player", pushPlayer);
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
