using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Threading;
using Newtonsoft.Json;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Misc;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.LyricProvider.Netease.Request;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.LyricProvider.Netease.Response;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.Misc;
using SearchOption = osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.Helper.SearchOption;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.LyricProvider.Netease;

public partial class NeteaseLyricProvider : LyricProvider
{
    private UrlEncoder? encoder;

    private string createRelativeLyricPath(WorkingBeatmap beatmap)
    {
        return $"custom/lyrics/new-beatmap-{beatmap.BeatmapSetInfo.ID}.json";
    }

    public override void Lookup(SearchOption option)
    {
        if (State.Value != SearchState.FuzzySearching)
            SetState(SearchState.Searching);

        var beatmap = option.Beatmap;

        if (beatmap == null)
        {
            Logging.Log("查询的谱面是null, 无法根据此Option查询歌词！");
            SetState(SearchState.Fail);
            return;
        }

        var onFinish = option.OnFinish;
        var onFail = option.OnFail;

        if (option.AllowLocalFile)
        {
            string abstractPath = createRelativeLyricPath(beatmap);
            string? fileFullPath = Storage.GetFullPath(abstractPath, true);

            if (fileFullPath != null)
            {
                try
                {
                    string content = File.ReadAllText(fileFullPath);

                    var deserializeObject = JsonConvert.DeserializeObject<LyricMeta>(content);

                    if (deserializeObject != null)
                    {
                        onFinish?.Invoke(deserializeObject);
                        SetState(SearchState.Success);
                        return;
                    }
                }
                catch (Exception e)
                {
                    Logging.LogError(e, "未能从本地文件获取歌词");
                    File.Delete(fileFullPath);
                }
            }
        }

        //TODO: 实现新版网易云API的查询
        if (false)
        {
            SetState(SearchState.Success);
            option.OnFinish?.Invoke(new APILyricResponseRoot());
            return;
        }

        encoder ??= UrlEncoder.Default;

        //处理之前的请求
        songLookupTokenSource?.Cancel();
        songLookupTokenSource = new CancellationTokenSource();

        lyricLookupTokenSource?.Cancel();
        lyricLookupTokenSource = null;

        currentSearchRequest?.Dispose();
        currentLyricRequest?.Dispose();

        //处理要搜索的歌名: "标题 艺术家"
        string title = beatmap.Metadata.GetTitle();
        string artist = option.NoArtist ? string.Empty : $" {beatmap.Metadata.GetArtist()}";
        string target = encoder.Encode($"{title}{artist}");

        var req = new APISearchRequest(target);

        req.Finished += () =>
        {
            var meta = RequestFinishMeta.From(req.ResponseObject, beatmap, onFinish, onFail, option.TitleSimiliarThreshold);
            meta.NoRetry = option.NoRetry;

            onSongSearchFinish(meta, req);
        };

        req.Failed += e =>
        {
            if (currentSearchRequest == req)
                SetState(SearchState.Fail);

            string message = "[LyricProcessor] 查询歌曲失败";

            if (e is HttpRequestException)
                message += ", 未能送达http请求, 请检查当前网络以及代理";

            Logging.LogError(e, message);
            onFail?.Invoke(e.ToString());
        };

        req.PerformAsync(songLookupTokenSource.Token).ConfigureAwait(false);

        currentSearchRequest = req;
    }

    private APISearchRequest? currentSearchRequest;
    private APILyricRequest? currentLyricRequest;

    private CancellationTokenSource? songLookupTokenSource;
    private CancellationTokenSource? lyricLookupTokenSource;

    /// <summary>
    /// 通过给定的网易云音乐ID搜索歌曲
    /// </summary>
    /// <param name="id">歌曲ID</param>
    /// <param name="beatmap"></param>
    /// <param name="onFinish"></param>
    /// <param name="onFail"></param>
    public void CreateDummyRequestResult(int id, WorkingBeatmap beatmap,
                                         Action<APILyricResponseRoot> onFinish,
                                         Action<string> onFail)
    {
        //处理之前的请求
        songLookupTokenSource?.Cancel();
        songLookupTokenSource = new CancellationTokenSource();

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

        var meta = RequestFinishMeta.From(fakeResponse, beatmap, onFinish, onFail, 0);
        meta.NoRetry = true;

        onSongSearchFinish(meta, null);
    }

    /// <summary>
    /// 当歌曲搜索请求完成后...
    /// </summary>
    /// <param name="meta"></param>
    /// <param name="searchRequest"></param>
    private void onSongSearchFinish(RequestFinishMeta meta, APISearchRequest? searchRequest)
    {
        if (!meta.Success)
        {
            //如果没成功，尝试使用标题重搜
            if (meta.SourceBeatmap != null && !meta.NoRetry)
            {
                var searchOption = SearchOption.FromRequestFinishMeta(meta);
                searchOption.NoArtist = true;
                searchOption.NoRetry = true;
                searchOption.AllowLocalFile = false;

                if (searchRequest != null && searchRequest == currentSearchRequest)
                    SetState(SearchState.FuzzySearching);

                //Logging.Log("精准搜索失败, 将尝试只搜索标题...");
                Lookup(searchOption);
            }
            else
            {
                if (searchRequest != null && searchRequest == currentSearchRequest)
                    SetState(SearchState.Fail);

                meta.OnFail?.Invoke("未搜索到对应歌曲!");
            }

            return;
        }

        float similiarPrecentage = meta.GetSimiliarPrecentage();

        Logging.Log($"Beatmap: '{meta.SourceBeatmap?.Metadata.GetTitle() ?? "???"}' <-> '{meta.GetNeteaseTitle()}' -> {similiarPrecentage} <-> {meta.TitleSimiliarThreshold}");

        if (similiarPrecentage >= meta.TitleSimiliarThreshold)
        {
            lyricLookupTokenSource?.Cancel();
            lyricLookupTokenSource = new CancellationTokenSource();

            //标题匹配，发送歌词查询请求
            var req = new APILyricRequest(meta.SongID);
            req.Finished += () =>
            {
                if (currentLyricRequest == req)
                    SetState(SearchState.Success);

                meta.OnFinish?.Invoke(req.ResponseObject);
            };
            req.Failed += e =>
            {
                if (currentLyricRequest == req)
                    SetState(SearchState.Fail);

                Logging.LogError(e, "获取歌词失败");
            };
            req.PerformAsync(lyricLookupTokenSource.Token).ConfigureAwait(false);

            currentLyricRequest = req;
        }
        else
        {
            //Logging.Log("标题匹配失败, 将不会继续搜索歌词...");
            this.SetState(SearchState.Fail);

            Logging.Log($"对 {meta.SourceBeatmap?.Metadata.GetTitle() ?? "未知谱面"} 的标题匹配失败：");
            Logging.Log($"Beatmap: '{meta.SourceBeatmap?.Metadata.GetTitle() ?? "???"}' <-> '{meta.GetNeteaseTitle()}' -> {similiarPrecentage} < {meta.TitleSimiliarThreshold}");

            meta.OnFail?.Invoke("标题匹配失败, 将不会继续搜索歌词...");
        }
    }

    public override void Save(IWorkingBeatmap beatmap, List<Lyric> lyrics)
    {
        base.Save(beatmap, lyrics);
    }

    #region 歌词读取、写入

    public void WriteLrcToFile(List<Lyric> lyrics, WorkingBeatmap working)
    {
        try
        {
            string target = createRelativeLyricPath(working);

            var saved = new LyricMeta
            {
                Lyrics = lyrics,
                Offset = 0
            };

            string serializeObject = JsonConvert.SerializeObject(saved);

            File.WriteAllText(Storage.GetFullPath(target, true), serializeObject);
        }
        catch (Exception e)
        {
            Logging.LogError(e, "写入歌词时发生了错误");
        }
    }

    #endregion
}
