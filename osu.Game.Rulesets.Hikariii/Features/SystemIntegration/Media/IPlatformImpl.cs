using System;
using osu.Game.Beatmaps;

namespace osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Media;

public interface IPlatformImpl
{
    /// <summary>
    /// Called when the system requested an audio seek.
    /// Called along with the offset, in millisecond.
    ///
    /// For platforms that don't use offset to seek, <see cref="HandleSetProgress"/> should be called instead.
    /// </summary>
    Action<double>? HandleSeek { get; set; }

    /// <summary>
    /// Called when the system requested to set the audio progress.
    /// Called along with the target time, in millisecond
    /// </summary>
    Action<double>? HandleSetProgress { get; set; }

    /// <summary>
    /// Called when the system required Play/Pause.
    /// Called along with whether to play.
    /// </summary>
    Action<bool>? HandlePlayPause { get; set; }

    /// <summary>
    /// Called when the system requested TogglePause
    /// </summary>
    public Action? HandleTogglePause { get; set; }

    /// <summary>
    /// Called when the system requested to play the next track
    /// </summary>
    Action? HandleNext { get; set; }

    /// <summary>
    /// Called when the system request to play the last track
    /// </summary>
    Action? HandlePrevious { get; set; }

    /// <summary>
    /// Called when the system request to change whether to loop
    /// </summary>
    Action<bool>? HandleLoopStatus { get; set; }

    /// <summary>
    /// Called when the system request to change whether to enable random play.
    /// </summary>
    Action<bool>? HandleShuffleStatus { get; set; }

    /// <summary>
    /// Update the current beatmap info to the system
    /// </summary>
    WorkingBeatmap Beatmap { set; }

    /// <summary>
    /// Update current track progress to the system
    /// </summary>
    double Progress { set; }

    /// <summary>
    /// Update the current track length to the system
    /// </summary>
    double TrackLength { set; }

    /// <summary>
    /// Update whether the track is running, to the system
    /// </summary>
    bool TrackRunning { set; }

    /// <summary>
    /// Update whether we're looping to the system
    /// </summary>
    bool TrackLooping { set; }

    /// <summary>
    /// Update whether we're playing random tracks to the system
    /// </summary>
    bool RandomTrackEnabled { set; }

    /// <summary>
    /// Update whether we allow media controls from the system
    /// </summary>
    bool AllowExternalControl { set; }
}
