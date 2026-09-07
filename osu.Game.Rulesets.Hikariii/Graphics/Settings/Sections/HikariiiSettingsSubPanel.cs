using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Overlays;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Extensions;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins;

namespace osu.Game.Rulesets.Hikariii.Graphics.Settings.Sections;

public partial class HikariiiSettingsSubPanel : SettingsSubPanel
{
    protected override Drawable CreateHeader()
    {
        return new SettingsHeader("Hikariii", "播放器和插件设置");
    }

    [BackgroundDependencyLoader]
    private void load(IHikariiiPluginManager manager)
    {
        foreach (var pl in manager.GetAllPluginProviders().Values.Where(pl => pl.GetSettingsEntries(manager.TryGetPluginConfigOrThrow<IPluginConfigManager>(pl)).Length > 0))
            AddSection(new PluginSettingsSubsection(pl));
    }
}
