using osu.Framework.Allocation;
using osu.Framework.Localisation;
using osu.Framework.Platform;
using osu.Game.Configuration;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.IGPlayer.Helper.Configuration;

namespace osu.Game.Rulesets.IGPlayer.Settings.Mf
{
    public partial class LinuxSection : SettingsSubsection
    {
        protected override LocalisableString Header { get; } = "Linux";

        [BackgroundDependencyLoader]
        private void load(MConfigManager config, OsuConfigManager osuConfig, GameHost host)
        {
            Add(new LinuxSettings());
            Add(new DBusSettings());
        }
    }
}
