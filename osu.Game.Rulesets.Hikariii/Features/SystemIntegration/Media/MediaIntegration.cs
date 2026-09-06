using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hikariii.Features.Configuration;
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

    private bool allowExternalControls;

    private void onControlStatusChange(bool allow)
    {
        allowExternalControls = allow;

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

    private void executeIfAllowControl(Action action)
    {
        if (ignoreMediaControlWhenFocused.Value && game.Window.IsActive.Value)
            return;

        if (allowExternalControls)
            action();
    }

    [Resolved]
    private OsuGame game { get; set; } = null!;

    private readonly BindableBool ignoreMediaControlWhenFocused = new(false);

    [BackgroundDependencyLoader]
    private void load(LLinGlobalConfigManager config)
    {
        config.BindWith(LLinGlobal.IgnoreMediaControlWhenFocused, ignoreMediaControlWhenFocused);

        var impl = selectImplementation();

        if (impl != null)
        {
            impl.HandleSeek += offset => executeIfAllowControl(() => MediaSource?.OnExternalSeek(offset));
            impl.HandleSetProgress += time => executeIfAllowControl(() => MediaSource?.OnExternalSetTime(time));

            impl.HandlePlayPause += b => executeIfAllowControl(() => MediaSource?.OnExternalPlayPause(b));
            impl.HandleNext += () => executeIfAllowControl(() => MediaSource?.OnExternalNext());
            impl.HandlePrevious += () => executeIfAllowControl(() => MediaSource?.OnExternalPrevious());

            impl.HandleTogglePause += () => executeIfAllowControl(() => MediaSource?.OnExternalTogglePause());

            impl.HandleLoopStatus += loop => executeIfAllowControl(() => MediaSource?.OnExternalLoopSet(loop));
            impl.HandleShuffleStatus += shuffle => executeIfAllowControl(() => MediaSource?.OnExternalShuffleSet(shuffle));
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

        if (OperatingSystem.IsWindows())
        {
            var windowsImpl = new WindowsPlatformImpl();
            LoadComponent(windowsImpl);
            AddInternal(windowsImpl);

            PlatformImpl = windowsImpl;
            return windowsImpl;
        }

        Logging.Log("System media integration is not available for this OS yet. Sorry!");
        return null;
    }
}
