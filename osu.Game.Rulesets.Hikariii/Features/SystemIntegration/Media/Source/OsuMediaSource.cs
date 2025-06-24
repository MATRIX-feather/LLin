using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Overlays;

namespace osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Media.Source;

public partial class OsuMediaSource : CompositeDrawable, IMediaSource
{
    [Resolved]
    private MusicController musicController { get; set; } = null!;

    [Resolved]
    private Bindable<WorkingBeatmap> beatmap { get; set; } = null!;

    public virtual void OnApply()
    {
        OnBeatmapChange?.Invoke(beatmap.Value);
        OnShuffleUpdate?.Invoke(musicController.Shuffle.Value);
        OnBeatmapChange?.Invoke(beatmap.Value);
    }

    public virtual void OnSwitchedAway()
    {
    }

    public virtual void OnExternalSeek(double offset)
    {
        double target = offset + musicController.CurrentTrack.CurrentTime;
        musicController.SeekTo(target);
        doUpdateProgress(target);
    }

    public virtual void OnExternalSetTime(double time)
    {
        musicController.SeekTo(time);
        doUpdateProgress(time);
    }

    public virtual void OnExternalPlayPause(bool play)
    {
        if (play)
            musicController.Play(requestedByUser: true);
        else
            musicController.Stop(requestedByUser: true);
    }

    public virtual void OnExternalNext()
    {
        musicController.NextTrack();
    }

    public virtual void OnExternalPrevious()
    {
        musicController.PreviousTrack();
    }

    public virtual void OnExternalLoopSet(bool looping)
    {
    }

    public virtual void OnExternalShuffleSet(bool shuffle)
    {
        musicController.Shuffle.Value = shuffle;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        trackLooping.BindValueChanged(v =>
        {
            OnLoopUpdate?.Invoke(v.NewValue);
        }, true);

        trackRunning.BindValueChanged(v =>
        {
            OnPlayPauseUpdate?.Invoke(v.NewValue);
        }, true);

        beatmap.BindValueChanged(v =>
        {
            trackLoaded.Value = false;
            OnBeatmapChange?.Invoke(v.NewValue);
        });

        musicController.Shuffle.BindValueChanged(v =>
        {
            OnShuffleUpdate?.Invoke(v.NewValue);
        });

        trackLoaded.BindValueChanged(v =>
        {
            if (!v.NewValue) return;

            var track = musicController.CurrentTrack;
            updateTrackLength(track.Length);
            doUpdateProgress(musicController.CurrentTrack.CurrentTime);
        });
    }

    protected override void LoadComplete()
    {
        progressUpdateLoop();
    }

    private readonly BindableBool trackLooping = new();
    private readonly BindableBool trackRunning = new();
    private readonly BindableBool trackLoaded = new();

    protected override void Update()
    {
        trackLooping.Value = musicController.CurrentTrack.Looping;
        trackRunning.Value = musicController.CurrentTrack.IsRunning;
        trackLoaded.Value = musicController.CurrentTrack.TrackLoaded;
    }

    private void progressUpdateLoop()
    {
        var track = musicController.CurrentTrack;
        doUpdateProgress(track.CurrentTime);

        this.Delay(1000).Schedule(progressUpdateLoop);
    }

    private void doUpdateProgress(double progress)
    {
        OnProgressUpdate?.Invoke(progress);
    }

    private void updateTrackLength(double length)
    {
        OnTrackLengthUpdate?.Invoke(length);
    }

    public Action<WorkingBeatmap>? OnBeatmapChange { get; set; }

    public Action<double>? OnProgressUpdate { get; set; }

    public Action<double>? OnTrackLengthUpdate { get; set; }
    public Action<bool>? OnPlayPauseUpdate { get; set; }
    public Action<bool>? OnLoopUpdate { get; set; }
    public Action<bool>? OnShuffleUpdate { get; set; }
}
