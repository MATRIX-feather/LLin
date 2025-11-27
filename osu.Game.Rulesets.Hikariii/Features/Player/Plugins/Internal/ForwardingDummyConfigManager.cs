using osu.Game.Rulesets.Hikariii.Features.Configuration;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Internal;

public class MForwardingDummyConfigManager(MConfigManager mConfigManager) : IPluginConfigManager
{
    private readonly MConfigManager mConfigManager = mConfigManager;
    public MConfigManager ConfigManager() => mConfigManager;

    public void Dispose()
    {
    }
}
