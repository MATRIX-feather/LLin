using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Threading;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Localisation;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Misc;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.Misc;
using osu.Game.Rulesets.IGPlayer.Localisation.LLin.Plugins;
using Component = osu.Framework.Graphics.Component;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.Helper
{
    public partial class LyricProcessor : Component
    {
        #region 获取状态

        public enum SearchState
        {
            [LocalisableDescription(typeof(CloudMusicStrings), nameof(CloudMusicStrings.SearchStateFail))]
            Fail,

            [LocalisableDescription(typeof(CloudMusicStrings), nameof(CloudMusicStrings.SearchStateSearching))]
            Searching,

            [LocalisableDescription(typeof(CloudMusicStrings), nameof(CloudMusicStrings.SearchStateFuzzySearching))]
            FuzzySearching,

            [LocalisableDescription(typeof(CloudMusicStrings), nameof(CloudMusicStrings.SearchStateSuccess))]
            Success
        }

        public readonly Bindable<SearchState> State = new Bindable<SearchState>();

        private void setState(SearchState newState)
        {
            State.Value = newState;
        }

        #endregion

        #region 歌词获取

        private APISearchRequest? currentSearchRequest;
        private APILyricRequest? currentLyricRequest;

        private CancellationTokenSource cancellationTokenSource = null!;

        private UrlEncoder? encoder;

        /// <summary>
        /// 通过给定的<see cref="SearchOption"/>>搜索歌曲
        /// </summary>
        /// <param name="searchOption"><see cref="SearchOption"/>></param>
        public void Search(SearchOption searchOption)
        {
            if (State.Value != SearchState.FuzzySearching)
                setState(SearchState.Searching);

            var beatmap = searchOption.Beatmap;

            if (beatmap == null)
            {
                setState(SearchState.Fail);
                return;
            }

            var onFinish = searchOption.OnFinish;
            var onFail = searchOption.OnFail;

            if (!searchOption.NoLocalFile)
            {
                var localLyrics = GetLocalLyrics(beatmap);

                if (localLyrics != null)
                {
                    setState(SearchState.Success);
                    onFinish?.Invoke(localLyrics);
                    return;
                }
            }

            //TODO: 实现新版网易云API的查询
            if (LyricPlugin.DisableCloudLookup)
            {
                setState(SearchState.Success);
                searchOption.OnFinish?.Invoke(new APILyricResponseRoot());
                return;
            }

            encoder ??= UrlEncoder.Default;

            //处理之前的请求
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = new CancellationTokenSource();

            currentSearchRequest?.Dispose();
            currentLyricRequest?.Dispose();

            //处理要搜索的歌名: "标题 艺术家"
            string title = searchOption.SearchMode == SearchMode.RomanisedTitle ? beatmap.Metadata.Title : beatmap.Metadata.TitleUnicode;
            string artist = searchOption.SearchMode == SearchMode.NoArtist ? string.Empty : beatmap.Metadata.GetArtist();
            string target = encoder.Encode($"{title} {artist}");

            var req = new APISearchRequest(target);

            req.Finished += () =>
            {
                var meta = RequestFinishMeta.From(req.ResponseObject, beatmap, onFinish, onFail, searchOption.SearchMode, searchOption.TitleSimilarThreshold);

                onSongSearchRequestFinish(meta, req);
            };

            req.Failed += e =>
            {
                if (currentSearchRequest == req)
                    setState(SearchState.Fail);

                string message = "[LyricProcessor] 查询歌曲失败";

                if (e is HttpRequestException)
                    message += ", 未能送达http请求, 请检查当前网络以及代理";

                Logging.LogError(e, message);
                onFail?.Invoke(e.ToString());
            };

            req.PerformAsync(cancellationTokenSource.Token).ConfigureAwait(false);

            currentSearchRequest = req;
        }

        public APILyricResponseRoot? GetLocalLyrics(WorkingBeatmap beatmap)
        {
            APILyricResponseRoot? deserializedObject = null;

            try
            {
                string path = storage.GetFullPath(lyricFilePath(beatmap), true);
                string content = File.ReadAllText(path);
                deserializedObject = JsonConvert.DeserializeObject<APILyricResponseRoot>(content);
            }
            catch
            {
                //忽略异常
            }

            return deserializedObject;
        }

        /// <summary>
        /// 通过给定的网易云音乐ID搜索歌曲
        /// </summary>
        /// <param name="id">歌曲ID</param>
        /// <param name="beatmap"></param>
        /// <param name="onFinish"></param>
        /// <param name="onFail"></param>
        public void SearchByNeteaseID(long id, WorkingBeatmap beatmap, Action<APILyricResponseRoot> onFinish, Action<string> onFail)
        {
            //处理之前的请求
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = new CancellationTokenSource();

            var fakeResponse = new APISearchResponseRoot
            {
                Result = new APISearchResultInfo
                {
                    SongCount = 1,
                    Songs = new List<APISongInfo>
                    {
                        new APISongInfo
                        {
                            ID = id
                        }
                    }
                }
            };

            var meta = RequestFinishMeta.From(fakeResponse, beatmap, onFinish, onFail, 0, 0);
            meta.NoRetry = true;

            onSongSearchRequestFinish(meta, null);
        }

        private static string lyricFilePath(WorkingBeatmap beatmap) => $"custom/lyrics/beatmap-{beatmap.BeatmapSetInfo.ID}.json";

        /// <summary>
        /// 当歌曲搜索请求完成后...
        /// </summary>
        /// <param name="meta"></param>
        /// <param name="searchRequest"></param>
        private void onSongSearchRequestFinish(RequestFinishMeta meta, APISearchRequest? searchRequest)
        {
            var sourceBeatmap = meta.SourceBeatmap;
            var songs = meta.SearchResponseRoot.Result?.Songs ?? [];

            songs.ForEach(s => s.CalculateSimilarPercentage(sourceBeatmap));

            var titleMatches = songs.Where(p => p.TitleSimilarPercentage >= meta.TitleSimilarThreshold)
                                    .OrderByDescending(p => p.TitleSimilarPercentage);
            var artistMatches = titleMatches.OrderByDescending(s => s.ArtistSimilarPercentage);
            var match = artistMatches.FirstOrDefault();
            string title = meta.SearchMode == SearchMode.RomanisedTitle ? sourceBeatmap.Metadata.Title : sourceBeatmap.Metadata.TitleUnicode;

            if (match != null)
            {
                if (match.ArtistSimilarPercentage >= (meta.SearchMode == SearchMode.NoArtist ? 0 : meta.TitleSimilarThreshold))
                {
                    Logging.Log($"Beatmap: '{title}' <-> '{match.Name}' -> {match.TitleSimilarPercentage} >= {meta.TitleSimilarThreshold}");
                }
            }

            if (match == null)
            {
                var searchMeta = SearchOption.FromRequestFinishMeta(meta);

                switch (meta.SearchMode)
                {
                    case SearchMode.Normal:
                        Logging.Log("尝试使用罗马音标题搜索");
                        searchMeta.SearchMode = SearchMode.RomanisedTitle;
                        searchMeta.NoLocalFile = true;

                        if (searchRequest != null && searchRequest == currentSearchRequest)
                            setState(SearchState.FuzzySearching);

                        Search(searchMeta);
                        break;

                    case SearchMode.RomanisedTitle:
                        searchMeta.SearchMode = SearchMode.NoArtist;
                        searchMeta.NoLocalFile = true;

                        if (searchRequest != null && searchRequest == currentSearchRequest)
                            setState(SearchState.FuzzySearching);

                        Search(searchMeta);
                        break;

                    case SearchMode.NoArtist:
                        meta.OnFail?.Invoke("标题匹配失败, 将不会继续搜索歌词...");
                        setState(SearchState.Fail);
                        break;
                }

                return;
            }

            var req = new APILyricRequest(match.ID);
            req.Finished += () =>
            {
                if (currentLyricRequest == req)
                    setState(SearchState.Success);

                meta.OnFinish?.Invoke(req.ResponseObject);
            };
            req.Failed += e =>
            {
                if (currentLyricRequest == req)
                    setState(SearchState.Fail);

                Logging.LogError(e, "获取歌词失败");
            };
            req.PerformAsync(cancellationTokenSource.Token).ConfigureAwait(false);

            currentLyricRequest = req;
        }

        #endregion

        #region 歌词读取、写入

        [Resolved]
        private Storage storage { get; set; } = null!;

        public void WriteLrcToFile(APILyricResponseRoot? responseRoot, WorkingBeatmap beatmap)
        {
            try
            {
                string serializedObject = JsonConvert.SerializeObject(responseRoot);
                File.WriteAllText(storage.GetFullPath(lyricFilePath(beatmap), true), serializedObject);
            }
            catch (Exception e)
            {
                Logging.LogError(e, "写入歌词时发生了错误");
            }
        }

        #endregion
    }
}
