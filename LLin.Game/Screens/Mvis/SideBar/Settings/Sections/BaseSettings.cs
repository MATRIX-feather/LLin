using LLin.Game.Configuration;
using LLin.Game.Screens.Mvis.Plugins;
using LLin.Game.Screens.Mvis.Plugins.Types;
using LLin.Game.Screens.Mvis.SideBar.Settings.Items;
using LLin.Game.Screens.Mvis.SideBar.Tabs;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace LLin.Game.Screens.Mvis.SideBar.Settings.Sections
{
    public class BaseSettings : Section
    {
        private readonly BindableFloat iR = new BindableFloat();
        private readonly BindableFloat iG = new BindableFloat();
        private readonly BindableFloat iB = new BindableFloat();

        public BaseSettings()
        {
            Title = "基本设置";
        }

        private Bindable<string> audioControlBindable;
        private Bindable<string> functionBarProviderBindable;

        [BackgroundDependencyLoader]
        private void load(MConfigManager config, MvisPluginManager pluginManager, MvisScreen mvisScreen)
        {
            config.BindWith(MSetting.MvisInterfaceRed, iR);
            config.BindWith(MSetting.MvisInterfaceGreen, iG);
            config.BindWith(MSetting.MvisInterfaceBlue, iB);

            var functionBarProviders = pluginManager.GetAllFunctionBarProviders();
            functionBarProviders.Insert(0, pluginManager.DummyFunctionBar);

            audioControlBindable = config.GetBindable<string>(MSetting.MvisCurrentAudioProvider);
            functionBarProviderBindable = config.GetBindable<string>(MSetting.MvisCurrentFunctionBar);

            Bindable<IProvideAudioControlPlugin> audioConfigBindable;
            Bindable<IFunctionBarProvider> functionBarConfigBindable;

            AddRange(new Drawable[]
            {
                new SettingsSliderPiece<float>
                {
                    Description = "界面主题色(红)",
                    Bindable = iR
                },
                new SettingsSliderPiece<float>
                {
                    Description = "界面主题色(绿)",
                    Bindable = iG
                },
                new SettingsSliderPiece<float>
                {
                    Description = "界面主题色(蓝)",
                    Bindable = iB
                },
                new ProviderSettingsPiece<IProvideAudioControlPlugin>
                {
                    Icon = FontAwesome.Solid.Bullseye,
                    Description = "音乐控制插件",
                    Bindable = audioConfigBindable = new Bindable<IProvideAudioControlPlugin>
                    {
                        Default = pluginManager.DefaultAudioController
                    },
                    Values = pluginManager.GetAllAudioControlPlugin()
                },
                new ProviderSettingsPiece<IFunctionBarProvider>
                {
                    Icon = FontAwesome.Solid.Bullseye,
                    Description = "底栏插件",
                    Bindable = functionBarConfigBindable = new Bindable<IFunctionBarProvider>
                    {
                        Default = pluginManager.DummyFunctionBar
                    },
                    Values = functionBarProviders
                },
                new SettingsEnumPiece<TabControlPosition>
                {
                    Icon = FontAwesome.Solid.Ruler,
                    Description = "TabControl位置",
                    Bindable = config.GetBindable<TabControlPosition>(MSetting.MvisTabControlPosition)
                },
                new SettingsSliderPiece<float>
                {
                    Icon = FontAwesome.Solid.SolarPanel,
                    Description = "背景模糊",
                    Bindable = config.GetBindable<float>(MSetting.MvisBgBlur),
                    DisplayAsPercentage = true
                },
                new SettingsSliderPiece<float>
                {
                    Icon = FontAwesome.Regular.Sun,
                    Description = "空闲时的背景亮度",
                    Bindable = config.GetBindable<float>(MSetting.MvisIdleBgDim),
                    DisplayAsPercentage = true
                },
                new SettingsTogglePiece
                {
                    Icon = FontAwesome.Regular.ArrowAltCircleUp,
                    Description = "置顶Proxy",
                    Bindable = config.GetBindable<bool>(MSetting.MvisStoryboardProxy),
                    TooltipText = "让所有Proxy显示在前景上方"
                },
                new SettingsTogglePiece
                {
                    Icon = FontAwesome.Solid.Clock,
                    Description = "启用背景动画",
                    Bindable = config.GetBindable<bool>(MSetting.MvisEnableBgTriangles),
                    TooltipText = "如果条件允许,播放器将会在背景显示动画"
                }
            });

            audioControlBindable.BindValueChanged(v =>
            {
                audioConfigBindable.Value = pluginManager.GetAudioControlByPath(audioControlBindable.Value);
            }, true);

            audioConfigBindable.BindValueChanged(v =>
            {
                if (v.NewValue == null)
                {
                    config.SetValue(MSetting.MvisCurrentAudioProvider, string.Empty);
                    return;
                }

                config.SetValue(MSetting.MvisCurrentAudioProvider, pluginManager.ToPath(v.NewValue));
            });

            functionBarProviderBindable.BindValueChanged(v =>
            {
                functionBarConfigBindable.Value = pluginManager.GetFunctionBarProviderByPath(functionBarProviderBindable.Value);
            }, true);

            functionBarConfigBindable.BindValueChanged(v =>
            {
                if (v.NewValue == null)
                {
                    config.SetValue(MSetting.MvisCurrentFunctionBar, string.Empty);
                    return;
                }

                config.SetValue(MSetting.MvisCurrentFunctionBar, pluginManager.ToPath(v.NewValue));
            });
        }

        private class ProviderSettingsPiece<T> : SettingsListPiece<T>
        {
            protected override string GetValueText(T newValue)
            {
                return (newValue as MvisPlugin)?.Name ?? "???";
            }
        }
    }
}
