using Humanizer;
using JetBrains.Annotations;
using LLin.Game.Graphics.Notifications;
using LLin.Game.Screens.Mvis;
using LLin.Game.Screens.Mvis.Plugins;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osu.Game.Collections;
using osu.Game.Input;
using osu.Game.Overlays;
using osu.Game.Screens;

namespace LLin.Game
{
    public class LLinGame : LLinGameBase
    {
        private OsuScreenStack screenStack;

        private DependencyContainer dependencies;
        private MvisPluginManager plManager;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        [NotNull]
        protected NotificationTray NotificationTray = new NotificationTray();

        [BackgroundDependencyLoader]
        private void load()
        {
            //错误信息发到通知
            Logger.NewEntry += entry =>
            {
                if (entry.Level < LogLevel.Important) return;

                NotificationLevel level;

                switch (entry.Level)
                {
                    case LogLevel.Important:
                        level = NotificationLevel.Warning;
                        break;

                    case LogLevel.Error:
                        level = NotificationLevel.Error;
                        break;

                    default:
                        level = NotificationLevel.Normal;
                        break;
                }

                Schedule(() => NotificationTray.Post(new SimpleNotification
                {
                    Text = entry.Message.Truncate(300),
                    Level = level
                }));
            };

            dependencies.CacheAs(this);

            dependencies.CacheAs(plManager = new MvisPluginManager());
            AddInternal(plManager);

            // Add your top-level game components here.
            // A screen stack and sample screen has been provided for convenience, but you can replace it if you don't want to use screens.
            Child = screenStack = new OsuScreenStack
            {
                RelativeSizeAxes = Axes.Both
            };

            screenStack.ScreenPushed += onScreenChanged;
            screenStack.ScreenExited += onScreenChanged;
        }

        private void onScreenChanged(IScreen lastscreen, IScreen newscreen)
        {
            if (newscreen == null)
                Exit();
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            loadAndCache(new DialogOverlay());
            loadAndCache(new IdleTracker(3000));
            loadAndCache(new ManageCollectionsDialog());
            loadAndCache(NotificationTray);

            screenStack.Push(new MvisScreen());
        }

        private void loadAndCache<T>(T target)
            where T : Drawable
        {
            Content.Add(target);
            dependencies.CacheAs(target);
        }
    }
}
