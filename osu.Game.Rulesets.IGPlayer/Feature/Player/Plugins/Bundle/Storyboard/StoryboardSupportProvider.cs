using osu.Game.Rulesets.IGPlayer.Feature.Player.Interfaces.Plugins;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.Storyboard
{
    public class StoryboardPluginProvider : LLinPluginProvider
    {
        public override LLinPlugin CreatePlugin => new BackgroundStoryBoardLoader();
    }
}
