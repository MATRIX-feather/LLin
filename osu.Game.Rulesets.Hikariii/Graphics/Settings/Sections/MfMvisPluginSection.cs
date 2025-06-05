using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Localisation;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;

namespace osu.Game.Rulesets.Hikariii.Graphics.Settings
{
    public partial class MfMvisPluginSection : SettingsSubsection
    {
        [BackgroundDependencyLoader]
        private void load(LLinPluginManager manager)
        {
            foreach (LLinPlugin pl in manager.GetAllPlugins(false).Where(pl => manager.GetSettingsFor(pl)?.Length > 0))
                Add(new PluginSettingsSubsection(pl));
        }

        protected override LocalisableString Header => "Hikariii播放器 - 设置和插件";
    }
}
