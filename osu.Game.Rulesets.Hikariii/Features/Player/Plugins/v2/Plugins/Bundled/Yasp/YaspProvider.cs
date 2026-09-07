using System;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SettingsItems;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Bundled.Yasp.Config;
using osu.Game.Rulesets.Hikariii.Localisation.LLin.Plugins;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Bundled.Yasp
{
    public class YaspProvider : IHikariiiPluginProvider
    {
        public string GetID() => "yasp";

        public IPluginConfigManager CreatePluginConfig(Storage storageAccess) => new YaspConfigManager(storageAccess, this);

        public Type GetPluginConfigType() => typeof(YaspConfigManager);

        public PluginDescription GetPluginDescription() => new("Yasp", "某人自己写的歌曲信息面板", ["MATRIX-feather"]);

        public DrawableHikariiiPlugin CreateDrawablePlugin() => new DrawableYaspPlugin(this);

        public SettingsEntry[] GetSettingsEntries(IPluginConfigManager ipcm)
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
    }
}
