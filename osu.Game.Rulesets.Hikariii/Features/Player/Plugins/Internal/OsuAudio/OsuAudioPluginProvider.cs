using osu.Framework.Localisation;
using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Configuration;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SettingsItems;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Localisation.LLin;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Internal.OsuAudio
{
    [LocalisableDescription(typeof(LLinPluginNameString), nameof(LLinPluginNameString.OsuMusicController))]
    internal class OsuAudioPluginProvider : LLinPluginProvider
    {
        private readonly MConfigManager config;

        public override PluginDescription GetDescription() => new(LLinPluginNameString.OsuMusicController, "集成 osu 音频控制", ["mfosu"]);

        internal OsuAudioPluginProvider(MConfigManager config)
        {
            this.config = config;
        }

        public override LLinPlugin CreatePlugin() => new OsuMusicControllerWrapper(this);

        public static readonly string ID = "osu_audio";

        public override string Identifier() => ID;

        public override SettingsEntry[] GetSettingEntries(IPluginConfigManager ipcm) => [];
        public override MForwardingDummyConfigManager CreateConfigManager(Storage storage) => new(config);
    }
}
