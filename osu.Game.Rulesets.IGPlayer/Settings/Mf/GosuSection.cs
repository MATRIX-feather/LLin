using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.IGPlayer.Helper.Configuration;

namespace osu.Game.Rulesets.IGPlayer.Settings.Mf;

public partial class GosuSection : SettingsSubsection
{
    protected override LocalisableString Header { get; } = "Gosu集成";

    [BackgroundDependencyLoader]
    private void load(MConfigManager config)
    {
        Children = new Drawable[]
        {
            new SettingsSlider<int>
            {
                Current = config.GetBindable<int>(MSetting.GosuMaximumCacheSize),
                LabelText = "缓存清理触发大小 (MiB)",
                TooltipText = "当文件缓存超过此大小时下次写入前将清理缓存"
            }
        };
    }
}
