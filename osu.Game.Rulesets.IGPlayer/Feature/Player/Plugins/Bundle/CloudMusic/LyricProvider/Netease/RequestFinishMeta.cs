using System;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Misc;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.Helper;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.LyricProvider.Netease.Response;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.LyricProvider.Netease
{
    public struct RequestFinishMeta
    {
        public APISearchResponseRoot SearchResponseRoot;
        public Action<APILyricResponseRoot>? OnFinish;
        public Action<string>? OnFail;

        /// <summary>
        /// 与此请求对应的<see cref="WorkingBeatmap"/>
        /// </summary>
        public WorkingBeatmap? SourceBeatmap;

        /// <summary>
        /// 标题匹配阈值，值越高要求越严格
        /// </summary>
        public float TitleSimiliarThreshold;

        /// <summary>
        /// 请求是否成功？
        /// </summary>
        public bool Success;

        /// <summary>
        /// 是否要重新搜索？
        /// </summary>
        public bool NoRetry;

        /// <summary>
        /// 歌曲ID，未搜到歌曲时返回-1
        /// </summary>
        public int SongID => (SearchResponseRoot.Result?.Songs?.First().ID ?? -1);

        /// <summary>
        /// 获取网易云歌曲标题和搜索标题的相似度
        /// </summary>
        /// <returns>相似度百分比</returns>
        public float GetSimiliarPrecentage()
        {
            string neteaseTitle = GetNeteaseTitle();
            string ourTitle = SourceBeatmap?.Metadata.GetTitle() ?? string.Empty;

            string source = neteaseTitle.Length > ourTitle.Length ? neteaseTitle : ourTitle;
            string target = neteaseTitle.Length > ourTitle.Length ? ourTitle : neteaseTitle;

            if (string.IsNullOrEmpty(neteaseTitle) || string.IsNullOrEmpty(ourTitle)) return 0;

            int distance = LevenshteinDistance.Compute(source, target);
            float precentage = 1 - (distance / (float)source.Length);

            return Math.Abs(precentage);
        }

        public string GetNeteaseTitle()
        {
            return SearchResponseRoot?.Result?.Songs?.First().Name ?? string.Empty;
        }

        /// <summary>
        /// 返回一个失败的<see cref="RequestFinishMeta"/>
        /// </summary>
        /// <param name="beatmap">和此Meta对应的<see cref="WorkingBeatmap"/>></param>
        /// <returns>通过参数构建的<see cref="RequestFinishMeta"/>></returns>
        public static RequestFinishMeta Fail(WorkingBeatmap? beatmap = null)
        {
            return new RequestFinishMeta
            {
                Success = false,

                OnFinish = null,
                OnFail = null,
                SearchResponseRoot = new APISearchResponseRoot(),

                SourceBeatmap = beatmap,
                NoRetry = true
            };
        }

        /// <summary>
        /// 通过给定的参数构建<see cref="RequestFinishMeta"/>>
        /// </summary>
        /// <param name="responseRoot"><see cref="APISearchResponseRoot"/>></param>
        /// <param name="sourceBeatmap">和此Meta对应的<see cref="WorkingBeatmap"/>></param>
        /// <param name="onFinish">完成时要进行的动作</param>
        /// <param name="onFail">失败时要进行的动作</param>
        /// <param name="titleSimiliarThreshold"><see cref="TitleSimiliarThreshold"/></param>
        /// <returns>通过参数构建的<see cref="RequestFinishMeta"/>></returns>
        public static RequestFinishMeta From(APISearchResponseRoot responseRoot, WorkingBeatmap? sourceBeatmap,
                                             Action<APILyricResponseRoot>? onFinish, Action<string>? onFail,
                                             float titleSimiliarThreshold)
        {
            return new RequestFinishMeta
            {
                OnFinish = onFinish,
                OnFail = onFail,
                SearchResponseRoot = responseRoot,
                Success = (responseRoot.Result?.Songs?.First().ID ?? -1) > 0,
                SourceBeatmap = sourceBeatmap,
                TitleSimiliarThreshold = titleSimiliarThreshold
            };
        }

        /// <summary>
        /// 通过给定的<see cref="RequestFinishMeta"/>>构建<see cref="SearchOption"/>
        /// <br/>
        /// 构建时将自动设置<see cref="Beatmap"/>、<see cref="OnFinish"/>和<see cref="OnFail"/>，其他属性保持默认值
        /// </summary>
        /// <param name="requestFinishMeta">目标Meta</param>
        /// <returns>通过参数构建的<see cref="SearchOption"/>></returns>
        public static SearchOption ToSearchOption(RequestFinishMeta requestFinishMeta)
        {
            return new SearchOption
            {
                Beatmap = requestFinishMeta.SourceBeatmap,

                OnFinish = lrcInfo =>
                {
                    requestFinishMeta.OnFinish?.Invoke(APILyricResponseRoot.FromLyricMeta(lrcInfo));
                },
                OnFail = requestFinishMeta.OnFail,

                TitleSimiliarThreshold = requestFinishMeta.TitleSimiliarThreshold
            };
        }
    }
}
