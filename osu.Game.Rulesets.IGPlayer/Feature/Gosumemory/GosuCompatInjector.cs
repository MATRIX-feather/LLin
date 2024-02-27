using System.IO;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Platform;
using osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Web;
using osuTK;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory
{
    public partial class GosuCompatInjector : Component
    {
        public TrackerHub? Updater { get; private set; }

        private Container getContainerFromGame(string containerName, OsuGame game)
        {
            foreach (var gameChild in game.Children)
            {
                if (gameChild?.Name == containerName && gameChild is Container childContainer)
                    return childContainer;
            }

            var child = new Container
            {
                Name = containerName
            };

            this.Schedule(() => game.Add(child));

            return child;
        }

        [Resolved(canBeNull: true)]
        private OsuGame? game { get; set; }

        private void initializeUpdater()
        {
            if (game == null)
            {
                Logging.Log("OsuGame is null, skipping gosu compat...");
                return;
            }

            var children = new Drawable[]
            {
                handler = new WebSocketLoader
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                },
                Updater = new TrackerHub(handler)
            };

            handler.OnServerStart += setupGosuStatics;

            var container = this.getContainerFromGame("mfosu Gosumemory compat container", game);
            container.AddRange(children);

            Logging.Log("Done initialize gosu compat!");
        }

        [Resolved]
        private Storage globalStorage { get; set; } = null!;

        private void setupGosuStatics(WebSocketLoader.GosuServer server)
        {
            Logging.Log("Setting up statics...");

            var staticsStorage = globalStorage.GetStorageForDirectory("gosu_statics");
            string? staticPath = staticsStorage.GetFullPath(".");

            if (staticPath == null || !Directory.Exists(staticPath))
            {
                Logging.Log("Null static path or it doesn't exists!");
                return;
            }

            server.SetStorage(globalStorage);

            DirectoryInfo dirInfo = new DirectoryInfo(staticPath);

            if (!dirInfo.Exists)
            {
                var newDirInfo = Directory.CreateDirectory(staticPath);

                if (!newDirInfo.Exists)
                {
                    Logging.Log("Unable to create statics directory, skipping...");
                    return;
                }
            }

            server.AddStaticContent(staticPath);
            Logging.Log("Done setting up static content!");
        }

        [BackgroundDependencyLoader]
        private void load(OsuGame? game)
        {
            Logging.Log("Gosu compat injector!");

            AlwaysPresent = true;

            this.Anchor = Anchor.Centre;
            this.Origin = Anchor.Centre;

            this.RelativeSizeAxes = Axes.Both;
            this.Size = new Vector2(0.6f);

            Logging.Log($"Updater is {Updater}");
            if (Updater == null)
                initializeUpdater();
        }

        private WebSocketLoader? handler;
    }
}
