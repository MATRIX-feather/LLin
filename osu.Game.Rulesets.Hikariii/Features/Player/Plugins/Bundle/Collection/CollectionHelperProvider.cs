using osu.Framework.Localisation;
using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SettingsItems;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Localisation.LLin;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection
{
    [LocalisableDescription(typeof(LLinPluginNameString), nameof(LLinPluginNameString.CollectionMusicController))]
    public class CollectionHelperProvider : LLinPluginProvider
    {
        public override LLinPlugin CreatePlugin() => new CollectionHelper(this);

        public override PluginDescription GetDescription() => new(LLinPluginNameString.CollectionMusicController, "将收藏夹作为歌单播放", ["mfosu"]);

        public override CollectionHelperConfigManager CreateConfigManager(Storage storage)
        {
            return new CollectionHelperConfigManager(storage);
        }

        public override SettingsEntry[] GetSettingEntries(IPluginConfigManager ipcm)
        {
            var config = (CollectionHelperConfigManager)ipcm;
            return
            [
                new BooleanSettingsEntry
                {
                    Name = "启用随机播放",
                    Bindable = config.GetBindable<bool>(CollectionSettings.EnableRandom)
                }
            ];
        }

        public override string Identifier() => "collection_helper";
    }
}
