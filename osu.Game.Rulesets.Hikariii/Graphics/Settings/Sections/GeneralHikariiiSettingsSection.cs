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
                TooltipText = "如果你不喜欢我们这样做, 可以关闭此选项。",
                Current = config.GetBindable<bool>(MSetting.InjectButtonToNewSongSelect)
            },
            new SettingsCheckbox
            {
                LabelText = "启用系统主题色支持",
                TooltipText = "如果当前系统受支持，则会基于系统主题色设定播放器的界面主题色",
                Current = config.GetBindable<bool>(MSetting.UsePlatformAccentColor)
            }
        ];
    }
}
