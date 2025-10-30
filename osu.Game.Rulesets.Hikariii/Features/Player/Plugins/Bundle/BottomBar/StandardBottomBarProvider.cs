using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.BottomBar
{
    public class StandardBottomBarProvider : LLinPluginProvider
    {
        public override LLinPlugin CreatePlugin() => new LegacyBottomBar(this);

        public override PluginDescription GetDescription() => new("标准底栏", "Hikariii 预装的标准底栏", ["mfosu"]);

        public override DefaultPluginConfigManager CreateConfigManager(Storage storage)
        {
            return new DefaultPluginConfigManager(storage);
        }

        public static readonly string ID = "standard_bottom_bar";

        public override string Identifier() => ID;
    }
}
