using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SettingsItems;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Storyboard.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Localisation.LLin;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Storyboard
{
    public class StoryboardPluginProvider : LLinPluginProvider
    {
        public override LLinPlugin CreatePlugin() => new BackgroundStoryBoardLoader(this);

        public override PluginDescription GetDescription() => new("故事版集成", "在背景显示谱面故事版", ["mfosu"]);

        public override SbLoaderConfigManager CreateConfigManager(Storage storage)
        {
            return new SbLoaderConfigManager(storage);
        }

        public override SettingsEntry[] GetSettingEntries(IPluginConfigManager ipcm)
        {
            var config = (SbLoaderConfigManager)ipcm;

            return
            [
                new BooleanSettingsEntry
                {
                    Name = LLinGenericStrings.EnablePlugin,
                    Bindable = config.GetBindable<bool>(SbLoaderSettings.EnableStoryboard)
                }
            ];
        }

        public override string Identifier() => "storyboard_loader";
    }
}
