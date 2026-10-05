using System;
using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SettingsItems;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Config;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Bundled.Storyboard
{
    public class BackgroundStoryboardProvider : IHikariiiPluginProvider
    {
        public const string ID = "background-storyboard";

        public string GetID() => ID;

        public IPluginConfigManager CreatePluginConfig(Storage storageAccess)
            => new DummyPluginConfigManager();

        public Type GetPluginConfigType()
            => typeof(DummyPluginConfigManager);

        public PluginDescription GetPluginDescription()
            => new("故事版集成", "在背景显示谱面故事版", ["mfosu"]);

        public DrawableHikariiiPlugin CreateDrawablePlugin()
            => new DrawableBackgroundStoryboard();

        public SettingsEntry[] GetSettingsEntries(IPluginConfigManager config)
            => [];
    }
}
