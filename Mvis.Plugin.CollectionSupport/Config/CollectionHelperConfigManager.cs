using osu.Framework.Platform;
using LLin.Game.Screens.Mvis.Plugins.Config;

namespace Mvis.Plugin.CollectionSupport.Config
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
