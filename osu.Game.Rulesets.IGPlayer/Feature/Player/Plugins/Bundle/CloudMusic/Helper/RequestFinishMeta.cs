using System;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.Misc;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.Helper
{
    public struct RequestFinishMeta
    {
        public APISearchResponseRoot SearchResponseRoot;
        public Action<APILyricResponseRoot>? OnFinish;
        public Action<string>? OnFail;

        /// <summary>
        /// 与此请求对应的<see cref="WorkingBeatmap"/>
        /// </summary>
        public WorkingBeatmap SourceBeatmap;

        /// <summary>
        /// 标题匹配阈值，值越高要求越严格
        /// </summary>
        public float TitleSimilarThreshold;

        /// <summary>
        /// 请求是否成功？
        /// </summary>
        public bool Success;

        /// <summary>
        /// 请求是否成功？
        /// </summary>
        public SearchMode SearchMode;

        /// <summary>
        /// 是否要重新搜索？
        /// </summary>
        public bool NoRetry;

        /// <summary>
        /// 通过给定的参数构建<see cref="RequestFinishMeta"/>>
        /// </summary>
        /// <param name="responseRoot"><see cref="APISearchResponseRoot"/>></param>
        /// <param name="sourceBeatmap">和此Meta对应的<see cref="WorkingBeatmap"/>></param>
        /// <param name="onFinish">完成时要进行的动作</param>
        /// <param name="onFail">失败时要进行的动作</param>
        /// <param name="searchMode"><see cref="SearchMode"/></param>
        /// <param name="titleSimiliarThreshold"><see cref="TitleSimilarThreshold"/></param>
        /// <returns>通过参数构建的<see cref="RequestFinishMeta"/>></returns>
        public static RequestFinishMeta From(APISearchResponseRoot responseRoot, WorkingBeatmap sourceBeatmap,
                                             Action<APILyricResponseRoot>? onFinish, Action<string>? onFail,
                                             SearchMode searchMode,
                                             float titleSimiliarThreshold)
        {
            return new RequestFinishMeta
            {
                OnFinish = onFinish,
                OnFail = onFail,
                SearchResponseRoot = responseRoot,
                SourceBeatmap = sourceBeatmap,
                SearchMode = searchMode,
                TitleSimilarThreshold = titleSimiliarThreshold
            };
        }
    }
}
