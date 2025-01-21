using osu.Game.Rulesets.Hikariii.Features.Configuration;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Internal.DummyAudio
{
    internal class DummyAudioPluginProvider : LLinPluginProvider
    {
        private readonly MConfigManager config;
        private readonly LLinPluginManager plmgr;

        internal DummyAudioPluginProvider(MConfigManager config, LLinPluginManager plmgr)
        {
            this.config = config;
            this.plmgr = plmgr;
        }

        public override LLinPlugin CreatePlugin => new DummyAudioPlugin(config, plmgr);
    }
}
