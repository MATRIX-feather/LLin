using osu.Game.Rulesets.Hikariii.Features.Configuration;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Internal.DummyBase
{
    internal class DummyBasePluginProvider : LLinPluginProvider
    {
        private readonly MConfigManager config;
        private readonly LLinPluginManager plmgr;

        internal DummyBasePluginProvider(MConfigManager config, LLinPluginManager plmgr)
        {
            this.config = config;
            this.plmgr = plmgr;
        }

        public override LLinPlugin CreatePlugin => new DummyBasePlugin(config, plmgr);
    }
}
