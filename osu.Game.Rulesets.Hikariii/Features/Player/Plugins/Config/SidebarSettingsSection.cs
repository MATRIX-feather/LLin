using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SettingsItems;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SideBar.Settings.Items;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SideBar.Settings.Sections;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.BuiltIn.Core;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config
{
    [Cached]
    public partial class NewPluginSettingsSection : Section
    {
        private readonly BindableFloat fillFlowMaxWidth = new BindableFloat();

        public int MaxRows { get; private set; } = 1;
        public Action<int>? OnNewMaxRows;
        private readonly SettingsEntry[] entries;

        public NewPluginSettingsSection(LocalisableString title, SettingsEntry[] entries)
        {
            Title = title;
            this.entries = entries;

            Alpha = 0.02f;
        }

        [BackgroundDependencyLoader]
        private void load(HikariiiCoreConfigManager config)
        {
            config.BindWith(HikariiiCoreSetting.SettingsMaxWidth, fillFlowMaxWidth);

            foreach (var se in entries)
            {
                var item = se.ToLLinSettingsItem();
                if (item != null) Add(item);
            }
        }

        protected override void LoadComplete()
        {
            fillFlowMaxWidth.BindValueChanged(v => FadeoutThen(200, () =>
                {
                    //bug: 直接设置FillFlow的Width可能并不会生效，灵异事件？
                    Width = v.NewValue;

                    int nmr = (int)Math.Floor(DrawWidth / (SettingsPieceBasePanel.SinglePanelWidth + 10f));

                    if (nmr != MaxRows)
                    {
                        MaxRows = nmr;
                        OnNewMaxRows?.Invoke(MaxRows);
                    }

                    FillFlow.AutoSizeDuration = 200;
                    FillFlow.AutoSizeEasing = Easing.OutQuint;
                })
                , true);

            base.LoadComplete();
        }
    }
}
