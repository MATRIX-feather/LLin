using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Graphics.SettingsItems;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Graphics.SideBar.Tabs;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Misc;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Config;
using osu.Game.Rulesets.IGPlayer.Helper.Configuration;
using osu.Game.Rulesets.IGPlayer.Localisation.LLin.Plugins;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Internal.DummyBase
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
                    Name = DummyBaseStrings.UIColorRed,
                    Bindable = config.GetBindable<float>(MSetting.MvisInterfaceRed),
                    KeyboardStep = 1
                },
                new NumberSettingsEntry<float>
                {
                    Name = DummyBaseStrings.UIColorGreen,
                    Bindable = config.GetBindable<float>(MSetting.MvisInterfaceGreen),
                    KeyboardStep = 1
                },
                new NumberSettingsEntry<float>
                {
                    Name = DummyBaseStrings.UIColorBlue,
                    Bindable = config.GetBindable<float>(MSetting.MvisInterfaceBlue),
                    KeyboardStep = 1
                },
                new ColorPreviewEntry(),
                new NumberSettingsEntry<float>
                {
                    Name = DummyBaseStrings.BgBlur,
                    Bindable = config.GetBindable<float>(MSetting.MvisBgBlur),
                    DisplayAsPercentage = true,
                    KeyboardStep = 0.01f,
                },
                new NumberSettingsEntry<float>
                {
                    Name = DummyBaseStrings.BgDimWhenIdle,
                    Bindable = config.GetBindable<float>(MSetting.MvisIdleBgDim),
                    DisplayAsPercentage = true,
                    KeyboardStep = 0.01f,
                },
                new EnumSettingsEntry<TabControlPosition>
                {
                    Name = DummyBaseStrings.TabControlPosition,
                    Bindable = config.GetBindable<TabControlPosition>(MSetting.MvisTabControlPosition)
                },
                new BooleanSettingsEntry
                {
                    Name = DummyBaseStrings.ProxyOnTop,
                    Bindable = config.GetBindable<bool>(MSetting.MvisStoryboardProxy),
                    Description = DummyBaseStrings.ProxyOnTopDesc
                },
                new BooleanSettingsEntry
                {
                    Name = DummyBaseStrings.BackgroundAnimations,
                    Bindable = config.GetBindable<bool>(MSetting.MvisEnableBgTriangles),
                    Description = DummyBaseStrings.BackgroundAnimationsDesc
                },
                listEntry = new ListSettingsEntry<TypeWrapper>
                {
                    Name = DummyBaseStrings.BottomBarPlugin,
                    Bindable = functionBarBindable
                },
                new BooleanSettingsEntry
                {
                    Name = DummyBaseStrings.PowersaveMode,
                    Bindable = config.GetBindable<bool>(MSetting.MvisAutoVSync),
                    Description = DummyBaseStrings.PowersaveModeDesc,
                    Icon = FontAwesome.Solid.Leaf
                },
                new BooleanSettingsEntry
                {
                    Name = DummyBaseStrings.TrianglesV2,
                    Bindable = config.GetBindable<bool>(MSetting.MvisUseTriangleV2),
                },
                new NumberSettingsEntry<float>
                {
                    Name = DummyBaseStrings.MaximumWidthForSettingsPanel,
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

        private class ColorPreviewEntry : SettingsEntry
        {
            public override Drawable ToSettingsItem() => new ColourPreviewer();

            public override Drawable? ToLLinSettingsItem() => null;
        }
    }
}
