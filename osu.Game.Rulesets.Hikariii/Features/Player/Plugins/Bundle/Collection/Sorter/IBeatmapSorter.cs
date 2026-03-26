using System.Collections.Generic;
using osu.Game.Beatmaps;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection.Sorter;

public interface IBeatmapSorter
{
    /// <summary>
    /// Sort the given beatmap list
    /// </summary>
    public void Sort(List<BeatmapInfo> beatmapInfos);

    /// <summary>
    /// Pick a beatmap from the given list... Mainly for implementing the *random pick* feature.
    /// </summary>
    /// <param name="beatmapInfos"></param>
    /// <returns>A beatmap pick from the list.</returns>
    public BeatmapInfo Pick(List<BeatmapInfo> beatmapInfos);
}
