using osu.Game.Rulesets.IGPlayer.Configuration;

namespace osu.Game.Rulesets.IGPlayer.Player.Plugins.Internal.DummyBase
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
