using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Game.Rulesets.Hikariii.Features.Configuration;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SettingsItems;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Misc;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Internal.DummyBase
{
    internal partial class DummyBasePlugin : LLinPlugin
    {
        internal DummyBasePlugin(MConfigManager config, LLinPluginManager plmgr)
        {
            HideFromPluginManagement = true;
            this.config = config;
            this.PluginManager = plmgr;

            Name = "基本设置";
            Version = LLinPluginManager.LatestPluginVersion;
        }

        private readonly MConfigManager config;

        public override SettingsEntry[] GetSettingEntries(IPluginConfigManager pluginConfigManager)
        {
            ListSettingsEntry<TypeWrapper> listEntry;
            var functionBarBindable = new Bindable<TypeWrapper>();

            var entries = new SettingsEntry[]
            {
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
                listEntry = new ListSettingsEntry<TypeWrapper>
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
                },
            };

            var plugins = PluginManager!.GetAllFunctionBarProviders();

            string currentFunctionBar = config.Get<string>(MSetting.MvisCurrentFunctionBar);

            foreach (var pl in plugins)
            {
                if (currentFunctionBar == PluginManager.ToPath(pl))
                {
                    functionBarBindable.Value = pl;
                }
            }

            listEntry.Values = plugins;
            functionBarBindable.Default = PluginManager.DefaultFunctionBarType;

            functionBarBindable.BindValueChanged(v =>
            {
                if (v.NewValue == null)
                {
                    config.SetValue(MSetting.MvisCurrentFunctionBar, string.Empty);
                    return;
                }

                var pl = v.NewValue;

                config.SetValue(MSetting.MvisCurrentFunctionBar, PluginManager.ToPath(pl));
            });

            return entries;
        }

        protected override Drawable CreateContent()
        {
            throw new System.NotImplementedException();
        }

        protected override bool OnContentLoaded(Drawable content)
        {
            throw new System.NotImplementedException();
        }

        protected override bool PostInit()
        {
            throw new System.NotImplementedException();
        }

        public override int Version { get; }
    }
}
