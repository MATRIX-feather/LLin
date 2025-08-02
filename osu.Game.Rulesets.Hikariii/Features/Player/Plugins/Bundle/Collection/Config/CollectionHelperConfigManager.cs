using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection.Config
{
    public class CollectionHelperConfigManager : PluginConfigManager<CollectionSettings>
    {
        public CollectionHelperConfigManager(Storage storage)
            : base(storage)
        {
        }

        protected override void InitialiseDefaults()
        {
            SetDefault(CollectionSettings.EnablePlugin, false);
            SetDefault(CollectionSettings.EnableRandom, false);
            base.InitialiseDefaults();
        }

        protected override string ConfigName => "CollectionSupport";
    }

    public enum CollectionSettings
    {
        EnablePlugin,
        EnableRandom
    }
}
