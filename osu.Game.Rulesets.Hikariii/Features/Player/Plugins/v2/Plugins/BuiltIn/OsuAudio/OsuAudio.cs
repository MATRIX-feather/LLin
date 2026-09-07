using System;
using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SettingsItems;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Localisation.LLin;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.BuiltIn.OsuAudio;

public class OsuAudio : IHikariiiPluginProvider
{
    public const string ID = "osu-audio";

    public string GetID() => ID;

    public IPluginConfigManager CreatePluginConfig(Storage storageAccess)
    {
        return new DummyPluginConfigManager();
    }

    public Type GetPluginConfigType() => typeof(DummyPluginConfigManager);

    public PluginDescription GetPluginDescription() => new(LLinPluginNameString.OsuMusicController, "集成 osu 音频控制", ["mfosu"]);

    public DrawableHikariiiPlugin CreateDrawablePlugin() => new DrawableOsuAudio();

    public SettingsEntry[] GetSettingsEntries(IPluginConfigManager config) => [];
}
