using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Hikariii.Features.Configuration;
using osuTK;

namespace osu.Game.Rulesets.Hikariii.Graphics.Settings.Sections;

public partial class GeneralHikariiiSettingsSection : SettingsSection
{
    public override Drawable CreateIcon()
    {
        return new SpriteIcon
        {
            Icon = FontAwesome.Solid.Atom,
            Size = new Vector2(18)
        };
    }

    public override LocalisableString Header => "通用";

    [BackgroundDependencyLoader]
    private void load(MConfigManager config)
    {
        Children =
        [
            new SettingsCheckbox
            {
                LabelText = "向新歌曲选择界面添加 Hikariii 入口",
                TooltipText = "受限于形式, Hikariii 只能在选歌屏幕被推送后等待一段时间再重新设定 Footer 按钮...\n如果你觉得不舒服, 可以关闭此选项。",
                Current = config.GetBindable<bool>(MSetting.InjectButtonToNewSongSelect)
            }
        ];
    }
}
