using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Web;
using osuTK;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory
{
    public partial class GosuCompatInjector : Component
    {
        public static Updater? Updater { get; private set; }

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
            var children = new Drawable[]
            {
                handler = new WsLoader
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                },
                Updater = new Updater(handler)
            };

            if (game != null)
            {
                var container = this.getContainerFromGame("mfosu Gosumemory compat container", game);

                if (container.Children.Count == 0)
                    container.AddRange(children);
            }
        }

        [BackgroundDependencyLoader]
        private void load(OsuGame? game)
        {
            this.Anchor = Anchor.Centre;
            this.Origin = Anchor.Centre;

            this.RelativeSizeAxes = Axes.Both;
            this.Size = new Vector2(0.6f);

            if (Updater == null)
                initializeUpdater();
        }

        private WsLoader? handler;
    }
}
