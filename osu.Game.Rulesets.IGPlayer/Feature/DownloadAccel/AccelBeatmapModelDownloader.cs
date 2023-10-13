using System;
using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Database;
using osu.Game.Online.API;
using osu.Game.Online.API.Requests;
using osu.Game.Overlays;
using osu.Game.Rulesets.IGPlayer.Feature.DownloadAccel.Extensions;
using osu.Game.Rulesets.IGPlayer.Helper.Configuration;

namespace osu.Game.Rulesets.IGPlayer.Feature.DownloadAccel;

public class AccelBeatmapModelDownloader : BeatmapModelDownloader
{
    public AccelBeatmapModelDownloader(IModelImporter<BeatmapSetInfo> beatmapImporter, IAPIProvider api)
        : base(beatmapImporter, api)
    {
    }

    protected override ArchiveDownloadRequest<IBeatmapSetInfo> CreateDownloadRequest(
        IBeatmapSetInfo set,
        bool minimiseDownloadSize)
    {
        return new AccelDownloadBeatmapSetRequest(set, minimiseDownloadSize);
    }

    public override ArchiveDownloadRequest<IBeatmapSetInfo>? GetExistingDownload(
        IBeatmapSetInfo model)
    {
        return this.CurrentDownloads.Find((Predicate<ArchiveDownloadRequest<IBeatmapSetInfo>>) (r => r.Model.OnlineID == model.OnlineID));
    }

    public void attachOsuGame(INotificationOverlay notificationOverlay)
    {
        this.PostNotification += notificationOverlay.Post;
    }

    private class AccelDownloadBeatmapSetRequest : DownloadBeatmapSetRequest
    {
        private readonly bool minimiseDownloadSize;

        public AccelDownloadBeatmapSetRequest(IBeatmapSetInfo set, bool noVideo)
            : base(set, noVideo)
        {
            this.minimiseDownloadSize = minimiseDownloadSize;
            var config = MConfigManager.GetInstance();

            var dict = new Dictionary<string, object>
            {
                ["BID"] = Model.OnlineID,
                ["NOVIDEO"] = minimiseDownloadSize
            };

            if (config.Get<string>(MSetting.AccelSource).TryParseAccelUrl(dict, out uri, out _, true)) return;

            Logging.LogError(new Exception("加速地址解析失败，请检查您的设置。"));
            this.Cancel();
        }

        private string getTarget() => $@"{(minimiseDownloadSize ? "novideo" : "full")}/{Model.OnlineID}";

        private readonly string uri;

        protected override string Target => getTarget();

        protected override string Uri => uri;
    }
}
