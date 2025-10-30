using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Configuration;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SettingsItems;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Internal.OsuAudio
{
    internal class OsuAudioPluginProvider : LLinPluginProvider
    {
        private readonly MConfigManager config;
        private SettingsEntry[]? entries;
        private readonly LLinPluginManager plmgr;

        public override PluginDescription GetDescription() => new("osu 音频集成", "集成 osu 音频控制", ["mfosu"]);

        internal OsuAudioPluginProvider(MConfigManager config, LLinPluginManager plmgr)
        {
            this.config = config;
            this.plmgr = plmgr;
        }

        public override void EarlyInitSettingEntries(IPluginConfigManager ipcm)
        {
            ListSettingsEntry<LLinPluginProvider> audioEntry;
            var audioPluginBindable = new Bindable<LLinPluginProvider>();

            entries =
            [
                new NumberSettingsEntry<double>
                {
                    Name = "播放速度",
                    Bindable = config.GetBindable<double>(MSetting.MvisMusicSpeed),
                    KeyboardStep = 0.01f,
                    DisplayAsPercentage = true,
                    //TransferValueOnCommit = true
                },
                new BooleanSettingsEntry
                {
                    Name = "调整音调",
                    Bindable = config.GetBindable<bool>(MSetting.MvisAdjustMusicWithFreq),
                    Description = "暂不支持调整故事版的音调"
                },
                new BooleanSettingsEntry
                {
                    Name = "夜核节拍器",
                    Bindable = config.GetBindable<bool>(MSetting.MvisEnableNightcoreBeat),
                    Description = "动次打次动次打次"
                },
                audioEntry = new ListSettingsEntry<LLinPluginProvider>
                {
                    Name = "音乐控制插件",
                    Bindable = audioPluginBindable
                }
            ];

            var plugins = plmgr.GetAllAudioControlPlugin();

            foreach (LLinPluginProvider provider in plugins.Where(provider => config.Get<string>(MSetting.MvisCurrentAudioProvider) == provider.Identifier()))
            {
                audioPluginBindable.Value = provider;
                break;
            }

            audioEntry.Values = plugins;
            audioPluginBindable.Default = this;

            audioPluginBindable.BindValueChanged(v =>
            {
                var pl = v.NewValue ?? this;

                config.SetValue(MSetting.MvisCurrentAudioProvider, pl.Identifier());
            });
        }

        public override LLinPlugin CreatePlugin() => new OsuMusicControllerWrapper(this);

        public static readonly string ID = "osu_audio";

        public override string Identifier() => ID;

        public override SettingsEntry[] GetSettingEntries(IPluginConfigManager ipcm) => entries ?? [];
        public override MForwardingDummyConfigManager CreateConfigManager(Storage storage) => new(config);
    }
}
