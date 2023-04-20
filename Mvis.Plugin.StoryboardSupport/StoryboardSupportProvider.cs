using osu.Game.Rulesets.IGPlayer.Player.Plugins;

namespace Mvis.Plugin.StoryboardSupport
{
    public class StoryboardPluginProvider : LLinPluginProvider
    {
        public override LLinPlugin CreatePlugin => new BackgroundStoryBoardLoader();
    }
}
