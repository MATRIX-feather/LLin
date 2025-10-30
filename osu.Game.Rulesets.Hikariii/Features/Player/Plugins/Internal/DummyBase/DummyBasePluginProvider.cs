using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Configuration;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SettingsItems;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.BottomBar;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Internal.FallbackFunctionBar;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Internal.DummyBase
{
    internal class DummyBasePluginProvider : LLinPluginProvider
    {
        private readonly MConfigManager config;
        private readonly LLinPluginManager plmgr;
        private SettingsEntry[]? entries;

        public override PluginDescription GetDescription() => new("播放器设置", "提供播放器基础设置", ["mfosu"]);

        internal DummyBasePluginProvider(MConfigManager config, LLinPluginManager plmgr)
        {
            this.config = config;
            this.plmgr = plmgr;
        }

        public override void EarlyInitSettingEntries(IPluginConfigManager ipcm)
        {
            ListSettingsEntry<LLinPluginProvider> listEntry;
            var functionBarBindable = new Bindable<LLinPluginProvider>();

            entries =
            [
                new NumberSettingsEntry<float>
                {
                    Name = "背景模糊",
                    Bindable = config.GetBindable<float>(MSetting.MvisBgBlur),
                    DisplayAsPercentage = true,
                    KeyboardStep = 0.01f,
                },
                new NumberSettingsEntry<float>
                {
                    Name = "空闲时的背景亮度",
                    Bindable = config.GetBindable<float>(MSetting.MvisIdleBgDim),
                    DisplayAsPercentage = true,
                    KeyboardStep = 0.01f,
                },
                new BooleanSettingsEntry
                {
                    Name = "启用背景动画",
                    Bindable = config.GetBindable<bool>(MSetting.MvisEnableBgTriangles),
                    Description = "如果条件允许,播放器将会在背景显示动画"
                },
                listEntry = new ListSettingsEntry<LLinPluginProvider>
                {
                    Name = "底栏插件",
                    Bindable = functionBarBindable
                },
                new BooleanSettingsEntry
                {
                    Name = "节能模式",
                    Bindable = config.GetBindable<bool>(MSetting.MvisAutoVSync),
                    Description = "启用后，将在进入播放器时自动启用垂直同步和单线程，并在退出时恢复进入前的状态",
                    Icon = FontAwesome.Solid.Leaf
                },
                new BooleanSettingsEntry
                {
                    Name = "使用新版三角粒子",
                    Bindable = config.GetBindable<bool>(MSetting.MvisUseTriangleV2),
                    Description = "可能不适合所有背景，仍需调教"
                },
                new BooleanSettingsEntry
                {
                    Name = "启用进、退场动画",
                    Description = "嗯...至少有人说挺炫酷的？",
                    Bindable = config.GetBindable<bool>(MSetting.MvisEnableAdvancedEnterLeaveAnimation)
                },
                new NumberSettingsEntry<float>
                {
                    Name = "播放器设置最大宽度",
                    Bindable = config.GetBindable<float>(MSetting.MvisPlayerSettingsMaxWidth),
                    DisplayAsPercentage = true,
                    KeyboardStep = 0.01f,
                    CommitOnMouseRelease = true
                }
            ];

            var plugins = plmgr.GetAllFunctionBarProviders();

            string currentFunctionBar = config.Get<string>(MSetting.MvisCurrentFunctionBar);

            foreach (LLinPluginProvider pl in plugins.Where(pl => currentFunctionBar == pl.Identifier()))
                functionBarBindable.Value = pl;

            listEntry.Values = plugins;
            functionBarBindable.Default = plmgr.AcquireProviderOrThrow<StandardBottomBarProvider>(StandardBottomBarProvider.ID);

            functionBarBindable.BindValueChanged(v =>
            {
                var pl = v.NewValue ?? plmgr.AcquireProviderOrThrow<FallbackFunctionBarProvider>(FallbackFunctionBarProvider.ID);

                config.SetValue(MSetting.MvisCurrentFunctionBar, pl.Identifier());
            });
        }

        public override LLinPlugin CreatePlugin() => new DummyBasePlugin(config, plmgr, this);

        public override IPluginConfigManager CreateConfigManager(Storage storage)
        {
            return new MForwardingDummyConfigManager(config);
        }

        public override SettingsEntry[] GetSettingEntries(IPluginConfigManager pluginConfigManager)
        {
            return entries ?? [];
        }

        public static readonly string ID = "dummy_base_options";

        public override string Identifier() => ID;
    }
}
