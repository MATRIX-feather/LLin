using Humanizer;
using JetBrains.Annotations;
using LLin.Game.Graphics.Containers;
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

        private ScreenContainer screenContainer;

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

            //依赖
            dependencies.CacheAs(this);

            // Add your top-level game components here.
            // A screen stack and sample screen has been provided for convenience, but you can replace it if you don't want to use screens.
            Children = new Drawable[]
            {
                screenContainer = new ScreenContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = screenStack = new OsuScreenStack
                    {
                        RelativeSizeAxes = Axes.Both
                    }
                }
            };

            screenStack.ScreenPushed += onScreenChanged;
            screenStack.ScreenExited += onScreenChanged;
        }

        private void onScreenChanged(IScreen lastscreen, IScreen newscreen)
        {
            if (newscreen == null)
                screenContainer.OnGameExit(Exit);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            addAndCache(plManager = new MvisPluginManager());
            addAndCache(new DialogOverlay());
            addAndCache(new IdleTracker(3000));
            addAndCache(new ManageCollectionsDialog());
            addAndCache(NotificationTray);

            OsuMusicController.NextTrack();

            screenStack.Push(new MvisScreen());
        }

        private void addAndCache<T>(T target)
            where T : Drawable
        {
            dependencies.CacheAs(target);
            Add(target);
        }
    }
}
