using System;
using osu.Framework.Localisation;
using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SettingsItems;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Localisation.LLin;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Bundled.FancyControls
{
    [LocalisableDescription(typeof(LLinPluginNameString), nameof(LLinPluginNameString.FancyControls))]
    public class FancyControls : IHikariiiPluginProvider
    {
        public const string ID = "fancy-controls";

        public string GetID() => ID;

        public IPluginConfigManager CreatePluginConfig(Storage storageAccess)
            => new DummyPluginConfigManager();

        public Type GetPluginConfigType()
            => typeof(DummyPluginConfigManager);

        public PluginDescription GetPluginDescription()
            => new(LLinPluginNameString.FancyControls, "更好看的预装底栏", ["mfosu"]);

        public DrawableHikariiiPlugin CreateDrawablePlugin()
            => new DrawableFancyControls();

        public SettingsEntry[] GetSettingsEntries(IPluginConfigManager config) => [];
    }
}
