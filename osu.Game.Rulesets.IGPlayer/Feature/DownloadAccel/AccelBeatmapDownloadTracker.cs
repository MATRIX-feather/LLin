using System;
using System.Reflection;
using osu.Framework.Allocation;
using osu.Framework.Logging;
using osu.Game.Beatmaps;
using osu.Game.Online;
using osu.Game.Online.API;
using osu.Game.Rulesets.IGPlayer.Helper.Injectors;

namespace osu.Game.Rulesets.IGPlayer.Feature.DownloadAccel;

public partial class AccelBeatmapDownloadTracker : BeatmapDownloadTracker
{
    public AccelBeatmapDownloadTracker(IBeatmapSetInfo trackedItem)
        : base(trackedItem)
    {
        if (trackedItem == null)
            throw new NullDependencyException("Tracked item may not be null");
    }

    protected override void LoadComplete()
    {
        Logger.Log($"Deps: {TrackedItem}");
        base.LoadComplete();

        var accelDownloader = PreviewTrackInjector.AccelBeatmapModelDownloader;
        if (accelDownloader == null) throw new NullDependencyException("Null dep");

        accelDownloader.DownloadBegan += this.downloadBegan;
        accelDownloader.DownloadFailed += this.downloadFailed;
    }

    private readonly BindingFlags flag = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic;

    private void downloadFailed(ArchiveDownloadRequest<IBeatmapSetInfo> obj)
    {
        var method = GetType().BaseType.GetMethod("attachDownload", flag);

        method.Invoke(this, new object?[]{ null });
    }

    private void downloadBegan(ArchiveDownloadRequest<IBeatmapSetInfo> obj)
    {
        var method = GetType().BaseType.GetMethod("attachDownload", flag);

        try
        {
            //Logger.Log($"Invoking Base Method! {method}", level: LogLevel.Important);
            method.Invoke(this, new object?[]{ obj });
        }
        catch (Exception e)
        {Logging.LogError(e, "???");
        }
    }
}
