using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.IGPlayer.Helper.Configuration;

namespace osu.Game.Rulesets.IGPlayer.Settings.Mf
{
    public partial class LinuxSettings : SettingsSubsection
    {
        protected override LocalisableString Header => "Linux集成";

        [BackgroundDependencyLoader]
        private void load(MConfigManager config)
        {
            SettingsCheckbox trayCheckbox;

            Children = new Drawable[]
            {
                new SettingsEnumDropdown<GamemodeActivateCondition>
                {
                    LabelText = "Gamemode启用条件",
                    TooltipText = "需要安装库：libgamemode0",
                    Current = config.GetBindable<GamemodeActivateCondition>(MSetting.Gamemode)
                },
                trayCheckbox = new SettingsCheckbox
                {
                    LabelText = "启用DBus系统托盘",
                    Current = config.GetBindable<bool>(MSetting.EnableTray)
                },
                new SettingsCheckbox
                {
                    LabelText = "允许通过DBus发送系统通知",
                    Current = config.GetBindable<bool>(MSetting.EnableSystemNotifications)
                },
                new SettingsTextBox
                {
                    LabelText = "托盘图标名称",
                    Current = config.GetBindable<string>(MSetting.TrayIconName)
                },
            };
        }
    }
}
