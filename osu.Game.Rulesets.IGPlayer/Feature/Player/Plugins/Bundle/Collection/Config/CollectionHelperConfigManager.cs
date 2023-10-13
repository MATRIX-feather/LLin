using osu.Framework.Platform;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Config;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.Collection.Config
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
            base.InitialiseDefaults();
        }

        protected override string ConfigName => "CollectionSupport";
    }

    public enum CollectionSettings
    {
        EnablePlugin
    }
}
