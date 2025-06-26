using System;
using System.IO;
using System.Linq;

#if WINDOWS
using LLin.OSIntegrations.Windows;
using osu.Game.Rulesets.Hikariii.Features.Player.Misc;
#endif

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Platform;
using osu.Game.Beatmaps;

namespace osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Media.Platform;

public partial class WindowsPlatformImpl : Drawable, IPlatformImpl
{
    public Action<double>? HandleSeek { get; set; }
    public Action<double>? HandleSetProgress { get; set; }
    public Action<bool>? HandlePlayPause { get; set; }
    public Action? HandleTogglePause { get; set; }
    public Action? HandleNext { get; set; }
    public Action? HandlePrevious { get; set; }
    public Action<bool>? HandleLoopStatus { get; set; }
    public Action<bool>? HandleShuffleStatus { get; set; }

    public double Progress
    {
        set
        {
#if WINDOWS
            windowsIntegration.Progress = value;
#endif
        }
    }

    public double TrackLength
    {
        set
        {
#if WINDOWS
            windowsIntegration.TrackLength = value;
#endif
        }
    }

    public bool TrackRunning
    {
        set
        {
#if WINDOWS
            windowsIntegration.TrackRunning = value;
#endif
        }
    }

    public bool TrackLooping
    {
        set
        {
#if WINDOWS
            windowsIntegration.TrackLooping = value;
#endif
        }
    }

    public bool RandomTrackEnabled
    {
        set
        {
#if WINDOWS
            windowsIntegration.Shuffle = value;
#endif
        }
    }

    public bool AllowExternalControl
    {
        set
        {
#if WINDOWS
            windowsIntegration.AllowExternalControl = value;
#endif
        }
    }

    public WorkingBeatmap Beatmap
    {
        set
        {
#if WINDOWS
            var info = value.BeatmapInfo;

            windowsIntegration.Title = info.Metadata.GetTitle().Title;
            windowsIntegration.Artist = info.Metadata.GetArtist();
            windowsIntegration.Album = info.DifficultyName;
            windowsIntegration.CoverUrl = resolveBeatmapCoverUrl(value);
#endif
        }
    }

    [Resolved]
    private Storage storage { get; set; } = null!;

    private string resolveBeatmapCoverUrl(WorkingBeatmap beatmap)
    {
        string path;
        string backgroundFilename = beatmap.BeatmapInfo.Metadata.BackgroundFile;

        if (!string.IsNullOrEmpty(backgroundFilename))
        {
            path = storage.GetFullPath("files")
                   + Path.DirectorySeparatorChar
                   + (beatmap.BeatmapSetInfo.GetPathForFile(beatmap.BeatmapInfo.Metadata.BackgroundFile)
                      ?? string.Empty);
        }
        else
        {
            string? target = storage.GetFiles("custom", "avatarlogo*")
                                    .FirstOrDefault(s => s.Contains("avatarlogo"));

            if (!string.IsNullOrEmpty(target))
                path = storage.GetFullPath(target);
            else
                return string.Empty;
        }

        var uri = new Uri(new Uri("file://"), path);
        Logging.Log("File URI is " + uri.ToString());
        return uri.ToString();
    }

    public WindowsPlatformImpl()
    {
        if (!OperatingSystem.IsWindows())
            throw new PlatformNotSupportedException("Platform not Windows, may not use SMTC integration.");
    }

#if WINDOWS
    private WindowsMediaIntegration windowsIntegration;

    [BackgroundDependencyLoader]
    private void load()
    {
        if (!OperatingSystem.IsWindows())
            throw new NotSupportedException("Platform not Windows, may not use SMTC integration.");

        windowsIntegration = new WindowsMediaIntegration();

        windowsIntegration.Play += () => Schedule(() => HandlePlayPause?.Invoke(true));
        windowsIntegration.Pause += () => Schedule(() => HandlePlayPause?.Invoke(false));
        windowsIntegration.Stop += () => Schedule(() => HandlePlayPause?.Invoke(false));
        windowsIntegration.TogglePause += () => Schedule(() => HandleTogglePause?.Invoke());

        windowsIntegration.Seek += offset => Schedule(() => HandleSeek?.Invoke(offset));
        windowsIntegration.SetPosition += targetTime => Schedule(() => HandleSetProgress?.Invoke(targetTime));

        windowsIntegration.Next += () => Schedule(() => HandleNext?.Invoke());
        windowsIntegration.Previous += () => Schedule(() => HandlePrevious?.Invoke());

        windowsIntegration.ShuffleChanged += v => Schedule(() => HandleShuffleStatus?.Invoke(v));

        Logging.Log("Successfully loaded windows integration!");
    }
#endif
}
