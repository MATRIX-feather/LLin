using osu.Framework.Graphics.Sprites;
using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SettingsItems;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Yasp.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Localisation.LLin;
using osu.Game.Rulesets.Hikariii.Localisation.LLin.Plugins;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Yasp
{
    public class YaspProvider : LLinPluginProvider
    {
        //在这里制定该Provider要提供的插件
        public override LLinPlugin CreatePlugin() => new YaspPlugin(this);

        public override PluginDescription GetDescription() => new("Yasp", "某人自己写的歌曲信息面板", ["MATRIX-feather"]);

        public override YaspConfigManager CreateConfigManager(Storage storage)
        {
            return new YaspConfigManager(storage);
        }

        public override SettingsEntry[] GetSettingEntries(IPluginConfigManager ipcm)
        {
            var config = (YaspConfigManager)ipcm;
            return
            [
                new NumberSettingsEntry<float>
                {
                    Icon = FontAwesome.Solid.ExpandArrowsAlt,
                    Name = YaspStrings.Scale,
                    Bindable = config.GetBindable<float>(YaspSettings.Scale),
                    DisplayAsPercentage = true,
                },
                new BooleanSettingsEntry
                {
                    Name = LLinGenericStrings.EnablePlugin,
                    Bindable = config.GetBindable<bool>(YaspSettings.EnablePlugin)
                },
                new BooleanSettingsEntry
                {
                    Name = YaspStrings.UseAvatarForCoverIICover,
                    Bindable = config.GetBindable<bool>(YaspSettings.CoverIIUseUserAvatar),
                },
                new EnumSettingsEntry<PanelType>
                {
                    Name = YaspStrings.PanelType,
                    Bindable = config.GetBindable<PanelType>(YaspSettings.PanelType)
                }
            ];
        }

        public override string Identifier() => "yasp";
    }
}
