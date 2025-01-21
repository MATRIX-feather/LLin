using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Storyboard
{
    public class StoryboardPluginProvider : LLinPluginProvider
    {
        public override LLinPlugin CreatePlugin => new BackgroundStoryBoardLoader();
    }
}
