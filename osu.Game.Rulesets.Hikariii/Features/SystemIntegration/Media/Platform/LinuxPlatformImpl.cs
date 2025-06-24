using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hikariii.Features.SystemIntegration.DBus;
using osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Mpris;
using Tmds.DBus;

namespace osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Media.Platform;

public partial class LinuxPlatformImpl : Drawable, IPlatformImpl
{
    [Resolved]
    private DBusIntegration dbusIntegration { get; set; } = null!;

    private readonly MprisPlayerService mprisPlayerService;

    public LinuxPlatformImpl()
    {
        mprisPlayerService = new MprisPlayerService();
    }

    [BackgroundDependencyLoader]
    private void load(Storage storage)
    {
        mprisPlayerService.Storage = storage;

        var manager = dbusIntegration.DBusManager;

        Logging.Log("DBus connect status is " + manager.ConnectionState);

        if (manager.ConnectionState == ConnectionState.Connected)
            registerService();
        else
            manager.OnConnected += registerService;
    }

    private void registerService()
    {
        Logging.Log("Registering MPRIS service...");
        var manager = dbusIntegration.DBusManager;
        manager.OnConnected -= registerService;
        manager.RegisterObject(mprisPlayerService).Wait();

        mprisPlayerService.Play += () => Schedule(() => HandlePlayPause?.Invoke(true));
        mprisPlayerService.Pause += () => Schedule(() => HandlePlayPause?.Invoke(false));
        mprisPlayerService.Stop += () => Schedule(() => HandlePlayPause?.Invoke(false));

        mprisPlayerService.Seek += offset => Schedule(() => HandleSeek?.Invoke(offset / 1000d));
        mprisPlayerService.SetPosition += targetTime => Schedule(() => HandleSetProgress?.Invoke(targetTime / 1000d));

        mprisPlayerService.Next += () => Schedule(() => HandleNext?.Invoke());
        mprisPlayerService.Previous += () => Schedule(() => HandlePrevious?.Invoke());

        mprisPlayerService.LoopChange += b => Schedule(() => HandleLoopStatus?.Invoke(b));
        mprisPlayerService.OnRandom += doRandom => Schedule(() => HandleShuffleStatus?.Invoke(doRandom));
    }

    public Action<double>? HandleSeek { get; set; }
    public Action<double>? HandleSetProgress { get; set; }
    public Action<bool>? HandlePlayPause { get; set; }
    public Action? HandleNext { get; set; }
    public Action? HandlePrevious { get; set; }
    public Action<bool>? HandleLoopStatus { get; set; }
    public Action<bool>? HandleShuffleStatus { get; set; }
    public Func<long>? HandleRequestProgress { get; set; }

    public WorkingBeatmap Beatmap
    {
        set => mprisPlayerService.Beatmap = value;
    }

    public double Progress
    {
        set => mprisPlayerService.Progress = (long)value * 1000;
    }

    public double TrackLength
    {
        set => mprisPlayerService.TrackLength = (long)value * 1000;
    }

    public bool TrackRunning
    {
        set => mprisPlayerService.TrackRunning = value;
    }

    public bool TrackLooping
    {
        set => mprisPlayerService.TrackLooping = value;
    }

    public bool RandomTrackEnabled
    {
        set => mprisPlayerService.Shuffle = value;
    }
}
