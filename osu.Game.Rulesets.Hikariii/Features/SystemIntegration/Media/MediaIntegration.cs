using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Overlays;
using osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Media.Platform;
using osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Media.Source;

namespace osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Media;

public partial class MediaIntegration : CompositeComponent
{
    protected readonly OsuMediaSource DefaultMediaSource = new();

    public IPlatformImpl? PlatformImpl { get; private set; }

    private IMediaSource? source;

    public IMediaSource? MediaSource
    {
        get => source;
        set
        {
            value ??= DefaultMediaSource;
            var last = source;

            if (last != null)
            {
                last.OnBeatmapChange -= onBeatmapChanged;
                last.OnPlayPauseUpdate -= onPlayPauseUpdate;

                last.OnLoopUpdate -= onLoopUpdate;
                last.OnShuffleUpdate -= onShuffleUpdate;

                last.OnProgressUpdate -= onProgressChange;
                last.OnTrackLengthUpdate -= onTrackLengthChange;

                value.OnControlStatusChange -= onControlStatusChange;

                last.OnSwitchedAway();
            }

            source = value;

            value.OnBeatmapChange += onBeatmapChanged;
            value.OnPlayPauseUpdate += onPlayPauseUpdate;

            value.OnLoopUpdate += onLoopUpdate;
            value.OnShuffleUpdate += onShuffleUpdate;

            value.OnProgressUpdate += onProgressChange;
            value.OnTrackLengthUpdate += onTrackLengthChange;

            value.OnControlStatusChange += onControlStatusChange;

            if (value is Drawable drawable)
            {
                if (drawable.IsLoaded)
                {
                    value.OnApply();
                }
                else
                {
                    drawable.OnLoadComplete += d =>
                    {
                        if (source == value)
                            value.OnApply();
                    };
                }
            }
            else
            {
                value.OnApply();
            }

            Logging.Log($"MediaHandler has been switched to {value}");
        }
    }

    private void onControlStatusChange(bool allow)
    {
        if (PlatformImpl != null)
            PlatformImpl.AllowExternalControl = allow;
    }

    private void onShuffleUpdate(bool shuffle)
    {
        if (PlatformImpl != null)
            PlatformImpl.RandomTrackEnabled = shuffle;
    }

    private void onLoopUpdate(bool looping)
    {
        if (PlatformImpl != null)
            PlatformImpl.TrackLooping = looping;
    }

    private void onPlayPauseUpdate(bool running)
    {
        if (PlatformImpl != null)
            PlatformImpl.TrackRunning = running;
    }

    private void onTrackLengthChange(double length)
    {
        if (PlatformImpl == null)
            return;

        PlatformImpl.TrackLength = length;
    }

    private void onProgressChange(double current)
    {
        if (PlatformImpl == null)
            return;

        PlatformImpl.Progress = current;
    }

    [BackgroundDependencyLoader]
    private void load(MusicController musicController)
    {
        var impl = selectImplementation();

        if (impl != null)
        {
            impl.HandleSeek += offset => MediaSource?.OnExternalSeek(offset);
            impl.HandleSetProgress += time => MediaSource?.OnExternalSetTime(time);

            impl.HandlePlayPause += b => MediaSource?.OnExternalPlayPause(b);
            impl.HandleNext += () => MediaSource?.OnExternalNext();
            impl.HandlePrevious += () => MediaSource?.OnExternalPrevious();

            impl.HandleTogglePause += () => MediaSource?.OnExternalTogglePause();

            impl.HandleLoopStatus += loop => MediaSource?.OnExternalLoopSet(loop);
            impl.HandleShuffleStatus += shuffle => MediaSource?.OnExternalShuffleSet(shuffle);
        }

        LoadComponent(DefaultMediaSource);
        AddInternal(DefaultMediaSource);
        MediaSource = DefaultMediaSource;
    }

    private void onBeatmapChanged(WorkingBeatmap beatmap)
    {
        if (PlatformImpl != null)
            PlatformImpl.Beatmap = beatmap;
    }

    private IPlatformImpl? selectImplementation()
    {
        if (OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
        {
            var linuxImpl = new LinuxPlatformImpl();
            LoadComponent(linuxImpl);
            AddInternal(linuxImpl);

            PlatformImpl = linuxImpl;

            return linuxImpl;
        }

        Logging.Log("System media integration is not available for this OS yet. Sorry!");
        return null;
    }
}
