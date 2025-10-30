using osu.Framework.Localisation;
using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Localisation.LLin;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.BottomBar
{
    [LocalisableDescription(typeof(LLinPluginNameString), nameof(LLinPluginNameString.StandardBottomBar))]
    public class StandardBottomBarProvider : LLinPluginProvider
    {
        public override LLinPlugin CreatePlugin() => new StandardBottomBar(this);

        public override PluginDescription GetDescription() => new(LLinPluginNameString.StandardBottomBar, "Hikariii 预装的标准底栏", ["mfosu"]);

        public override DefaultPluginConfigManager CreateConfigManager(Storage storage)
        {
            return new DefaultPluginConfigManager(storage);
        }

        public static readonly string ID = "standard_bottom_bar";

        public override string Identifier() => ID;
    }
}
