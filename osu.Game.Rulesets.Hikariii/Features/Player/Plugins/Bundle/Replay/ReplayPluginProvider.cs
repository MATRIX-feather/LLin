using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SettingsItems;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Replay.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Localisation.LLin;
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
        return new ReplayConfigManager(storage);
    }

    public override SettingsEntry[] GetSettingEntries(IPluginConfigManager ipcm)
    {
        var config = (ReplayConfigManager)ipcm;
        return
        [
            new BooleanSettingsEntry
            {
                Name = LLinGenericStrings.EnablePlugin,
                Bindable = config.GetBindable<bool>(ReplaySettings.EnablePlugin)
            },
            new EnumSettingsEntry<AutoplayPreference>
            {
                Name = BackgroundReplayStrings.AutoplayPreference,
                Bindable = config.GetBindable<AutoplayPreference>(ReplaySettings.UseAutoplay)
            }
        ];
    }
}
