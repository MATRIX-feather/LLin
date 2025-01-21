using osu.Framework.Platform;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config
{
    public class DefaultPluginConfigManager : PluginConfigManager<DefaultSettings>
    {
        public DefaultPluginConfigManager(Storage storage)
            : base(storage)
        {
        }

        protected override string ConfigName => "unknown";
    }

    public enum DefaultSettings
    {
    }
}
