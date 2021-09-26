using LLin.Game.Screens.Mvis;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Screens;

namespace LLin.Game
{
    public class LLinGame : LLinGameBase
    {
        private OsuScreenStack screenStack;

        [BackgroundDependencyLoader]
        private void load()
        {
            // Add your top-level game components here.
            // A screen stack and sample screen has been provided for convenience, but you can replace it if you don't want to use screens.
            Child = screenStack = new OsuScreenStack
            {
                RelativeSizeAxes = Axes.Both
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            screenStack.Push(new MvisScreen());
        }
    }
}
