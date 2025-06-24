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

    private IMediaSource? handler;

    public IMediaSource? MediaHandler
    {
        get => handler;
        set
        {
            value ??= DefaultMediaSource;
            var last = handler;

            if (last != null)
            {
                last.OnBeatmapChange -= onBeatmapChanged;
                last.OnPlayPauseUpdate -= onPlayPauseUpdate;

                last.OnLoopUpdate -= onLoopUpdate;
                last.OnShuffleUpdate -= onShuffleUpdate;

                last.OnProgressUpdate -= onProgressChange;
                last.OnTrackLengthUpdate -= onTrackLengthChange;

                last.OnSwitchedAway();
            }

            handler = value;

            value.OnBeatmapChange += onBeatmapChanged;
            value.OnPlayPauseUpdate += onPlayPauseUpdate;

            value.OnLoopUpdate += onLoopUpdate;
            value.OnShuffleUpdate += onShuffleUpdate;

            value.OnProgressUpdate += onProgressChange;
            value.OnTrackLengthUpdate += onTrackLengthChange;

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
                        if (handler == value)
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
            impl.HandleSeek += offset => MediaHandler?.OnExternalSeek(offset);
            impl.HandleSetProgress += time => MediaHandler?.OnExternalSetTime(time);

            impl.HandlePlayPause += b => MediaHandler?.OnExternalPlayPause(b);
            impl.HandleNext += () => MediaHandler?.OnExternalNext();
            impl.HandlePrevious += () => MediaHandler?.OnExternalPrevious();

            impl.HandleLoopStatus += loop => MediaHandler?.OnExternalLoopSet(loop);
            impl.HandleShuffleStatus += shuffle => MediaHandler?.OnExternalShuffleSet(shuffle);
        }

        LoadComponent(DefaultMediaSource);
        AddInternal(DefaultMediaSource);
        MediaHandler = DefaultMediaSource;
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
