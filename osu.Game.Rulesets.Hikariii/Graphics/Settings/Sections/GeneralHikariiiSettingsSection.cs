using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Hikariii.Features.Configuration;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Internal.DummyBase;
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
            new SettingsItemV2(new FormCheckBox
            {
                Caption = "向新歌曲选择界面添加 Hikariii 入口",
                HintText = "如果你不喜欢我们这样做, 可以关闭此选项。",
                Current = config.GetBindable<bool>(MSetting.InjectButtonToNewSongSelect)
            }),
            new SettingsItemV2(new FormCheckBox
            {
                Caption = "启用系统主题色支持",
                HintText = "如果当前系统受支持，则会基于系统主题色设定播放器的界面主题色",
                Current = config.GetBindable<bool>(MSetting.UsePlatformAccentColor)
            }),
            new SettingsItemV2(new FormCheckBox
            {
                Caption = "启用勿扰模式支持",
                HintText = "如果当前系统受支持，则会在游玩时启用系统的勿扰模式",
                Current = config.GetBindable<bool>(MSetting.EnableOSDoNotDisturbWhenPlaying)
            }),
            new SettingsItemV2(new FormCheckBox
            {
                Caption = "取得焦点时不要响应系统媒体按键",
                HintText = "当窗口取得焦点时，Hikariii 将不会对系统媒体按键做出响应\n修复一些平台上按下媒体按键会响应两次的问题",
                Current = config.GetBindable<bool>(MSetting.IgnoreMediaControlWhenFocused)
            }),
            new SettingsItemV2(new FormSliderBar<float>()
            {
                Caption = "界面主题色（红）",
                HintText = "若没有平台主题色可用，将根据这些设置设定界面主体色",
                Current = config.GetBindable<float>(MSetting.MvisInterfaceRed),
                KeyboardStep = 1f
            }),
            new SettingsItemV2(new FormSliderBar<float>
            {
                Caption = "界面主题色（绿）",
                HintText = "若没有平台主题色可用，将根据这些设置设定界面主体色",
                Current = config.GetBindable<float>(MSetting.MvisInterfaceGreen),
                KeyboardStep = 1f
            }),
            new SettingsItemV2(new FormSliderBar<float>
            {
                Caption = "界面主题色（蓝）",
                HintText = "若没有平台主题色可用，将根据这些设置设定界面主体色",
                Current = config.GetBindable<float>(MSetting.MvisInterfaceBlue),
                KeyboardStep = 1f
            }),
            new ColourPreviewer()
        ];
    }
}
