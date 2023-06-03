namespace osu.Game.Rulesets.IGPlayer.Player.Plugins.Bundle.Storyboard
{
    public class StoryboardPluginProvider : LLinPluginProvider
    {
        public override LLinPlugin CreatePlugin => new BackgroundStoryBoardLoader();
    }
}
