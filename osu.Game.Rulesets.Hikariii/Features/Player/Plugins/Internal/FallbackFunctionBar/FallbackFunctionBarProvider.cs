using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Internal.FallbackFunctionBar;

public class FallbackFunctionBarProvider : LLinPluginProvider
{
    public static readonly string ID = "fallback_function_bar";

    public override PluginDescription GetDescription() => new("后备底栏", "当没有其他底栏能用时会用他", ["mfosu"]);

    public override LLinPlugin CreatePlugin() => new FallbackFunctionBar(this);

    public override string Identifier() => ID;

    public override IPluginConfigManager CreateConfigManager(Storage storage)
    {
        return new DefaultPluginConfigManager(storage);
    }
}
