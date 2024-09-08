using System;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.LyricProvider;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.Helper
{
    public struct SearchOption
    {
        /// <summary>
        /// 和此SearchOption对应的<see cref="WorkingBeatmap"/>
        /// </summary>
        public WorkingBeatmap? Beatmap;

        public Action<LyricMeta>? OnFinish;
        public Action<string>? OnFail;

        /// <summary>
        /// 是否要不带艺术家搜索？
        /// </summary>
        public bool NoArtist;

        /// <summary>
        /// 失败后是否禁止重试？
        /// </summary>
        public bool NoRetry;

        /// <summary>
        /// 是否允许从本地文件查询？
        /// </summary>
        public bool AllowLocalFile;

        /// <summary>
        /// 标题匹配阈值，值越高要求越严格
        /// </summary>
        public float TitleSimiliarThreshold;

        /// <summary>
        /// 通过给定的参数构建<see cref="SearchOption"/>.
        /// </summary>
        /// <param name="sourceBeatmap">目标<see cref="WorkingBeatmap"/>></param>
        /// <param name="noLocalFile"><see cref="NoLocalFile"/></param>
        /// <param name="onFinish">完成时要进行的动作</param>
        /// <param name="onFail">失败时要进行的动作</param>
        /// <param name="titleSimiliarThreshold"><see cref="TitleSimiliarThreshold"/></param>
        /// <returns>通过参数构建的<see cref="SearchOption"/>></returns>
        public static SearchOption From(WorkingBeatmap sourceBeatmap, bool allowLocalFile,
                                        Action<LyricMeta>? onFinish, Action<string> onFail,
                                        float titleSimiliarThreshold)
        {
            return new SearchOption
            {
                Beatmap = sourceBeatmap,

                OnFinish = onFinish,
                OnFail = onFail,

                AllowLocalFile = allowLocalFile,

                TitleSimiliarThreshold = titleSimiliarThreshold
            };
        }
    }
}
