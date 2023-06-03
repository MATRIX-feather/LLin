using osu.Framework.Platform;
using osu.Game.Rulesets.IGPlayer.Player.Plugins.Config;

namespace osu.Game.Rulesets.IGPlayer.Player.Plugins.Bundle.Storyboard.Config
{
    public class SbLoaderConfigManager : PluginConfigManager<SbLoaderSettings>
    {
        public SbLoaderConfigManager(Storage storage)
            : base(storage)
        {
        }

        protected override void InitialiseDefaults()
        {
            SetDefault(SbLoaderSettings.EnableStoryboard, true);
        }

        protected override string ConfigName => "StoryboardSupport";
    }

    public enum SbLoaderSettings
    {
        EnableStoryboard
    }
}
