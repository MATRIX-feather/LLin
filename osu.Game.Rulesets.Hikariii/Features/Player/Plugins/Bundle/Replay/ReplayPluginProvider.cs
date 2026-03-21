using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Replay.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Localisation.LLin.Plugins;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Replay;

public class ReplayPluginProvider : LLinPluginProvider
{
    public override LLinPlugin CreatePlugin()
    {
        return new BackgroundReplayPlugin(this);
    }

    public override PluginDescription GetDescription()
    {
        return new PluginDescription(LLinPluginNameString.BackgroundReplay, LLinPluginNameString.BackgroundReplayDescription, []);
    }

    public override string Identifier()
    {
        return "replay";
    }

    public override IPluginConfigManager CreateConfigManager(Storage storage)
    {
        return new ReplayConfigManager();
    }
}
