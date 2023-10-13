using osu.Framework.Graphics.Containers;
using osu.Framework.Timing;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.SandboxToPanel.RulesetComponents.Screens.Visualizer.Components.MusicHelpers
{
    public partial class RateAdjustableContainer : Container
    {
        public double Rate
        {
            get => clock.Rate;
            set => clock.Rate = value;
        }

        private readonly StopwatchClock clock;

        public RateAdjustableContainer()
        {
            ProcessCustomClock = true;
            Clock = new FramedClock(clock = new StopwatchClock());
            clock.Start();
        }
    }
}
