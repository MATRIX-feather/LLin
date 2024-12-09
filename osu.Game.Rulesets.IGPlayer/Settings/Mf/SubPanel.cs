using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Overlays;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Interfaces.Plugins;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Config;

namespace osu.Game.Rulesets.IGPlayer.Settings.Mf;

public partial class SubPanel : SettingsSubPanel
{
    protected override Drawable CreateHeader()
    {
        return new SettingsHeader("Hikariii", "播放器和插件设置");
    }

    [BackgroundDependencyLoader]
    private void load(LLinPluginManager manager)
    {
        foreach (LLinPlugin pl in manager.GetAllPlugins(false).Where(pl => manager.GetSettingsFor(pl)?.Length > 0))
            AddSection(new PluginSettingsSubsection(pl));
    }
}
