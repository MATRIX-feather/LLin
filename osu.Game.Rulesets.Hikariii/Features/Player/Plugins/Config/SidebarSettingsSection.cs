using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Hikariii.Features.Configuration;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SideBar.Settings.Items;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SideBar.Settings.Sections;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config
{
    [Cached]
    public partial class NewPluginSettingsSection : Section
    {
        private readonly LLinPluginProvider provider;

        private readonly BindableFloat fillFlowMaxWidth = new BindableFloat();

        public NewPluginSettingsSection(LLinPluginProvider plugin)
        {
            this.provider = plugin;
            Title = plugin.GetDescription().Name;

            Alpha = 0.02f;
        }

        [BackgroundDependencyLoader]
        private void load(LLinPluginManager pluginManager, MConfigManager config)
        {
            config.BindWith(MSetting.MvisPlayerSettingsMaxWidth, fillFlowMaxWidth);

            foreach (var se in pluginManager.GetSettingsFor(provider))
            {
                var item = se.ToLLinSettingsItem();
                if (item != null) Add(item);
            }
        }

        public int MaxRows { get; private set; } = 1;
        public Action<int>? OnNewMaxRows;

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
