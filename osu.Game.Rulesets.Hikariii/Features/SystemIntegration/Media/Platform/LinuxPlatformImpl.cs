using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using M.DBus;
using M.DBus.Services.Mpris;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hikariii.Features.Player.Misc;
using osu.Game.Rulesets.Hikariii.Features.SystemIntegration.DBus;
using Tmds.DBus.Protocol;

namespace osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Media.Platform;

public partial class LinuxPlatformImpl : Drawable, IPlatformImpl
{
    [Resolved]
    private DBusIntegration dbusIntegration { get; set; } = null!;

    private MprisService? mprisPlayerService;

    [BackgroundDependencyLoader]
    private void load()
    {
        var session = dbusIntegration.AcquireNewSession();
        session.OnConnected += registerService;
        session.Connect().Wait();
    }

    private void registerService(DBusSession session)
    {
        if (session == null)
            throw new ArgumentNullException(nameof(session));

        Logging.Log("Registering MPRIS service...");

        var connection = session.CurrentConnection!;

        mprisPlayerService = new MprisService(connection);
        mprisPlayerService.register(connection);

        session.RequestServiceName("org.mpris.MediaPlayer2.mfosu_hikariii").Wait();

        mprisPlayerService.Play += () => Schedule(() => HandlePlayPause?.Invoke(true));
        mprisPlayerService.Pause += () => Schedule(() => HandlePlayPause?.Invoke(false));
        mprisPlayerService.Stop += () => Schedule(() => HandlePlayPause?.Invoke(false));
        mprisPlayerService.TogglePause += () => Schedule(() => HandleTogglePause?.Invoke());

        mprisPlayerService.Seek += offset => Schedule(() => HandleSeek?.Invoke(offset / 1000d));
        mprisPlayerService.SetPosition += targetTime => Schedule(() => HandleSetProgress?.Invoke(targetTime / 1000d));

        mprisPlayerService.Next += () => Schedule(() => HandleNext?.Invoke());
        mprisPlayerService.Previous += () => Schedule(() => HandlePrevious?.Invoke());

        //mprisPlayerService.LoopChange += b => Schedule(() => HandleLoopStatus?.Invoke(b));
        //mprisPlayerService.OnRandom += doRandom => Schedule(() => HandleShuffleStatus?.Invoke(doRandom));
    }

    public Action<double>? HandleSeek { get; set; }
    public Action<double>? HandleSetProgress { get; set; }
    public Action<bool>? HandlePlayPause { get; set; }
    public Action? HandleTogglePause { get; set; }
    public Action? HandleNext { get; set; }
    public Action? HandlePrevious { get; set; }
    public Action<bool>? HandleLoopStatus { get; set; }
    public Action<bool>? HandleShuffleStatus { get; set; }
    public Func<long>? HandleRequestProgress { get; set; }

    public WorkingBeatmap Beatmap
    {
        set
        {
            if (mprisPlayerService == null)
                return;

            var info = value.BeatmapInfo;
            var metadata = mprisPlayerService.Metadata;

            Debug.Assert(metadata != null);

            metadata["xesam:artist"] = Variant.FromArray([info.Metadata.GetArtist()]);
            metadata["xesam:title"] = info.Metadata.GetTitle().Title;
            metadata["xesam:album"] = info.DifficultyName;
            metadata["xesam:audioBPM"] = info.BPM;

            metadata["mpris:artUrl"] = resolveBeatmapCoverUrl(value);
            metadata["mpris:trackid"] = new ObjectPath("/not/implemented/yet");

            mprisPlayerService.Metadata = metadata;
        }
    }

    [Resolved]
    private Storage storage { get; set; } = null!;

    private string resolveBeatmapCoverUrl(WorkingBeatmap beatmap)
    {
        string body;
        string backgroundFilename = beatmap?.BeatmapInfo.Metadata.BackgroundFile;

        if (!string.IsNullOrEmpty(backgroundFilename))
        {
            body = storage?.GetFullPath("files")
                   + Path.DirectorySeparatorChar
                   + (beatmap.BeatmapSetInfo.GetPathForFile(beatmap.BeatmapInfo.Metadata?.BackgroundFile)
                      ?? string.Empty);

            Logging.Log("COVER PATH IS " + body);
        }
        else
        {
            string? target = storage?.GetFiles("custom", "avatarlogo*")
                                    .FirstOrDefault(s => s.Contains("avatarlogo"));

            if (!string.IsNullOrEmpty(target))
                body = storage.GetFullPath(target);
            else
                return string.Empty;
        }

        return $"file://{body}";
    }

    public double Progress
    {
        set
        {
            if (mprisPlayerService == null)
                return;

            mprisPlayerService.Progress = (long)value * 1000;
        }
    }

    public double TrackLength
    {
        set
        {
            if (mprisPlayerService == null)
                return;

            mprisPlayerService.TrackLength = (long)value * 1000;
        }
    }

    public bool TrackRunning
    {
        set
        {
            if (mprisPlayerService == null)
                return;

            mprisPlayerService.TrackRunning = value;
        }
    }

    public bool TrackLooping
    {
        set
        {
            if (mprisPlayerService == null)
                return;

            mprisPlayerService.TrackLooping = value;
        }
    }

    public bool RandomTrackEnabled
    {
        set
        {
            if (mprisPlayerService == null)
                return;

            mprisPlayerService.Shuffle = value;
        }
    }
}
