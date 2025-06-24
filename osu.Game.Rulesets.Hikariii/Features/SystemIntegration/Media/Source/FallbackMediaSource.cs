using System;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Graphics.Textures;
using osu.Game.Beatmaps;

namespace osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Media.Source;

public class FallbackMediaSource : IMediaSource
{
    private Action<double>? onProgressUpdate;

    [Resolved]
    private AudioManager audioManager { get; set; } = null!;

    [Resolved]
    private TextureStore textures { get; set; } = null!;

    public void OnApply()
    {
        var dummy = new DummyWorkingBeatmap(audioManager, textures)
        {
            Metadata =
            {
                Title = "暂无可用媒体源",
                Artist = "暂无可用媒体源",
            }
        };

        OnBeatmapChange?.Invoke(dummy);
    }

    public void OnSwitchedAway()
    {
    }

    public void OnExternalSeek(double offset)
    {
    }

    public void OnExternalSetTime(double time)
    {
    }

    public void OnExternalPlayPause(bool play)
    {
    }

    public void OnExternalNext()
    {
    }

    public void OnExternalPrevious()
    {
    }

    public void OnExternalLoopSet(bool looping)
    {
    }

    public void OnExternalShuffleSet(bool shuffle)
    {
    }

    public Action<WorkingBeatmap>? OnBeatmapChange { get; set; }

    public Action<double>? OnProgressUpdate { get; set; }

    public Action<double>? OnTrackLengthUpdate { get; set; }
    public Action<bool>? OnPlayPauseUpdate { get; set; }
    public Action<bool>? OnLoopUpdate { get; set; }
    public Action<bool>? OnShuffleUpdate { get; set; }
}
