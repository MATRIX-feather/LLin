using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Graphics.SettingsItems;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Misc;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Config;
using osu.Game.Rulesets.IGPlayer.Helper.Configuration;
using osu.Game.Rulesets.IGPlayer.Localisation.LLin.Plugins;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Internal.DummyAudio
{
    internal partial class DummyAudioPlugin : LLinPlugin
    {
        public override LocalisableString PluginName => BuiltinAudioStrings.PluginName;

        internal DummyAudioPlugin(MConfigManager config, LLinPluginManager plmgr)
        {
            HideFromPluginManagement = true;
            this.config = config;
            this.PluginManager = plmgr;

            Name = "音频";
            Version = LLinPluginManager.LatestPluginVersion;
        }

        private SettingsEntry[]? entries;
        private readonly MConfigManager config;

        public override SettingsEntry[] GetSettingEntries(IPluginConfigManager pluginConfigManager)
        {
            if (entries == null)
            {
                ListSettingsEntry<TypeWrapper> audioEntry;
                var audioPluginBindable = new Bindable<TypeWrapper>();

                entries = new SettingsEntry[]
                {
                    new NumberSettingsEntry<double>
                    {
                        Name = BuiltinAudioStrings.PlaySpeed,
                        Bindable = config.GetBindable<double>(MSetting.MvisMusicSpeed),
                        KeyboardStep = 0.01f,
                        DisplayAsPercentage = true,
                        //TransferValueOnCommit = true
                    },
                    new BooleanSettingsEntry
                    {
                        Name = BuiltinAudioStrings.AdjustPitchTitle,
                        Bindable = config.GetBindable<bool>(MSetting.MvisAdjustMusicWithFreq),
                        Description = BuiltinAudioStrings.AdjustPitchDesc
                    },
                    new BooleanSettingsEntry
                    {
                        Name = BuiltinAudioStrings.NightCoreTitle,
                        Bindable = config.GetBindable<bool>(MSetting.MvisEnableNightcoreBeat),
                        Description = BuiltinAudioStrings.NightCoreDesc
                    },
                    audioEntry = new ListSettingsEntry<TypeWrapper>
                    {
                        Name = BuiltinAudioStrings.AudioControlPlugin,
                        Bindable = audioPluginBindable
                    }
                };

                var plugins = PluginManager!.GetAllAudioControlPlugin();

                foreach (var pl in plugins)
                {
                    if (config.Get<string>(MSetting.MvisCurrentAudioProvider) == PluginManager.ToPath(pl))
                    {
                        audioPluginBindable.Value = pl;
                        break;
                    }
                }

                audioEntry.Values = plugins;
                audioPluginBindable.Default = PluginManager.DefaultAudioControllerType;

                audioPluginBindable.BindValueChanged(v =>
                {
                    if (v.NewValue == null)
                    {
                        config.SetValue(MSetting.MvisCurrentAudioProvider, string.Empty);
                        return;
                    }

                    var pl = v.NewValue;

                    config.SetValue(MSetting.MvisCurrentAudioProvider, PluginManager.ToPath(pl));
                });
            }

            return entries;
        }

        protected override Drawable CreateContent()
        {
            throw new System.NotImplementedException();
        }

        protected override bool OnContentLoaded(Drawable content)
        {
            throw new System.NotImplementedException();
        }

        protected override bool PostInit()
        {
            throw new System.NotImplementedException();
        }

        public override int Version { get; }
    }
}
