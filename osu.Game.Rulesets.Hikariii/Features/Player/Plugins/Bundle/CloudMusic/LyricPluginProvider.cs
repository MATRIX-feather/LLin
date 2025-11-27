using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SettingsItems;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.CloudMusic.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Localisation.LLin;
using osu.Game.Rulesets.Hikariii.Localisation.LLin.Plugins;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.CloudMusic
{
    public class LyricPluginProvider : LLinPluginProvider
    {
        //在这里制定该Provider要提供的插件
        public override LLinPlugin CreatePlugin() => new LyricPlugin(this);

        public override PluginDescription GetDescription() => new("歌词集成", "从网易云音乐下载歌词（如果可用）", ["mfosu"]);

        public override LyricConfigManager CreateConfigManager(Storage storage)
        {
            return new LyricConfigManager(storage);
        }

        public override SettingsEntry[] GetSettingEntries(IPluginConfigManager ipcm)
        {
            var config = (LyricConfigManager)ipcm;

            return
            [
                new BooleanSettingsEntry
                {
                    Name = LLinGenericStrings.EnablePlugin,
                    Bindable = config.GetBindable<bool>(LyricSettings.EnablePlugin)
                },
                new BooleanSettingsEntry
                {
                    Icon = FontAwesome.Solid.Save,
                    Name = CloudMusicStrings.SaveLyricOnDownloadedMain,
                    Bindable = config.GetBindable<bool>(LyricSettings.SaveLrcWhenFetchFinish),
                    Description = CloudMusicStrings.SaveLyricOnDownloadedSub
                },
                new BooleanSettingsEntry
                {
                    Icon = FontAwesome.Solid.FillDrip,
                    Name = CloudMusicStrings.DisableShader,
                    Bindable = config.GetBindable<bool>(LyricSettings.NoExtraShadow)
                },
                new NumberSettingsEntry<float>
                {
                    Name = CloudMusicStrings.LyricFadeInDuration,
                    Bindable = config.GetBindable<float>(LyricSettings.LyricFadeInDuration)
                },
                new NumberSettingsEntry<float>
                {
                    Name = CloudMusicStrings.LyricFadeOutDuration,
                    Bindable = config.GetBindable<float>(LyricSettings.LyricFadeOutDuration)
                },
                new BooleanSettingsEntry
                {
                    Name = CloudMusicStrings.LyricAutoScrollMain,
                    Bindable = config.GetBindable<bool>(LyricSettings.AutoScrollToCurrent)
                },
                new ListSettingsEntry<Anchor>
                {
                    Icon = FontAwesome.Solid.Anchor,
                    Name = CloudMusicStrings.LocationDirection,
                    Bindable = config.GetBindable<Anchor>(LyricSettings.LyricDirection),
                    Values =
                    [
                        Anchor.TopLeft,
                        Anchor.TopCentre,
                        Anchor.TopRight,
                        Anchor.CentreLeft,
                        Anchor.Centre,
                        Anchor.CentreRight,
                        Anchor.BottomLeft,
                        Anchor.BottomCentre,
                        Anchor.BottomRight
                    ]
                },
                new NumberSettingsEntry<float>
                {
                    Name = CloudMusicStrings.PositionX,
                    Bindable = config.GetBindable<float>(LyricSettings.LyricPositionX),
                    DisplayAsPercentage = true
                },
                new NumberSettingsEntry<float>
                {
                    Name = CloudMusicStrings.PositionY,
                    Bindable = config.GetBindable<float>(LyricSettings.LyricPositionY),
                    DisplayAsPercentage = true
                },
                new NumberSettingsEntry<float>
                {
                    Name = "歌曲相似度阈值",
                    Bindable = config.GetBindable<float>(LyricSettings.TitleSimilarThreshold),
                    DisplayAsPercentage = true,
                    Description = "网易云搜出的歌词有时不一定能和当前谱面匹配。 阈值越高, 对网易云返回的搜索结果检查越严格，搜索成功率也就越低"
                },
                new BooleanSettingsEntry
                {
                    Name = "启用用户定义",
                    Bindable = config.GetBindable<bool>(LyricSettings.EnableUserDefinitions),
                    Description = "启用用户定义后，将通过设置的URL获取相关设置来优先匹配本地谱面ID以提供更准确的歌词查询"
                },
                new BooleanSettingsEntry
                {
                    Name = "输出定义到日志",
                    Bindable = config.GetBindable<bool>(LyricSettings.OutputDefinitionInLogs),
                    Description = "更新定义时输出内容到日志中，可以在某些情况下帮助查找相关信息"
                },
                new StringSettingsEntry
                {
                    Name = "用户定义文件URL",
                    Bindable = config.GetBindable<string>(LyricSettings.UserDefinitionURL),
                    Description = "将通过此URL拉取用户定义配置"
                }
            ];
        }

        public override string Identifier() => "lyrics";
    }
}
