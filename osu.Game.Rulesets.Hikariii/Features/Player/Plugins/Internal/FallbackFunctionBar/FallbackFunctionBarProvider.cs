using osu.Framework.Localisation;
using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Localisation.LLin;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Internal.FallbackFunctionBar;

[LocalisableDescription(typeof(LLinPluginNameString), nameof(LLinPluginNameString.FallbackFunctionBar))]
public partial class FallbackFunctionBarProvider : LLinPluginProvider
{
    public static readonly string ID = "fallback_function_bar";

    public override PluginDescription GetDescription() => new(LLinPluginNameString.FallbackFunctionBar, "当没有其他底栏能用时会用他", ["mfosu"]);

    public override LLinPlugin CreatePlugin() => new FallbackFunctionBar(this);

    public override string Identifier() => ID;

    public override IPluginConfigManager CreateConfigManager(Storage storage)
    {
        return new DummyPluginConfigManager();
    }
}
