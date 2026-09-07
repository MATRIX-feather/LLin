using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Config;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Storyboard.Config
{
    public class SbLoaderConfigManager : PluginConfigManager<SbLoaderSettings>
    {
        public SbLoaderConfigManager(Storage storage)
            : base(storage, null) //todo: FIXME fix null provider
        {
        }

        protected override void InitialiseDefaults()
        {
            SetDefault(SbLoaderSettings.EnableStoryboard, true);
        }
    }

    public enum SbLoaderSettings
    {
        EnableStoryboard
    }
}
