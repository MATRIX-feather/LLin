using System.ComponentModel;
using osu.Framework.Configuration;
using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Config;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Yasp.Config
{
    public class YaspConfigManager : PluginConfigManager<YaspSettings>
    {
        public YaspConfigManager(Storage storage)
            : base(storage, null) //todo: FIXME fix null provider
        {
        }

        /// <summary>
        /// 在这里初始化默认值, 更多用法请见 <see cref="ConfigManager"/>
        /// </summary>
        protected override void InitialiseDefaults()
        {
            SetDefault(YaspSettings.Scale, 1, 0, 5f);
            SetDefault(YaspSettings.EnablePlugin, true);
            SetDefault(YaspSettings.PanelType, PanelType.Classic);
            SetDefault(YaspSettings.CoverIIUseUserAvatar, false);

            base.InitialiseDefaults();
        }

        //配置文件名，已更改的值将在"plugin-{ConfigName}.ini"中保存
    }

    public enum YaspSettings
    {
        Scale,
        EnablePlugin,

        PanelType,
        CoverIIUseUserAvatar
    }

    public enum PanelType
    {
        [Description("经典")]
        Classic,

        [Description("封面")]
        SongCover,

        CoverII
    }
}
