using System;
using osu.Game.Beatmaps;

namespace osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Media;

public interface IMediaSource
{
    void OnApply();
    void OnSwitchedAway();

    void OnExternalSeek(double offset);
    void OnExternalSetTime(double time);

    void OnExternalPlayPause(bool play);
    void OnExternalNext();
    void OnExternalPrevious();

    void OnExternalLoopSet(bool looping);
    void OnExternalShuffleSet(bool shuffle);

    Action<WorkingBeatmap>? OnBeatmapChange { get; set; }

    Action<double>? OnProgressUpdate { get; set; }

    Action<double>? OnTrackLengthUpdate { get; set; }

    Action<bool>? OnPlayPauseUpdate { get; set; }

    Action<bool>? OnLoopUpdate { get; set; }

    Action<bool>? OnShuffleUpdate { get; set; }
}
