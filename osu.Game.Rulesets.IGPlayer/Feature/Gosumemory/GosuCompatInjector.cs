using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
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
                Logging.Log("OsuGame is null, returning...");
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

            var container = this.getContainerFromGame("mfosu Gosumemory compat container", game);
            container.AddRange(children);

            Logging.Log("Added gosu compat!");
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
