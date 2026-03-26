using System.Collections.Generic;
using osu.Game.Beatmaps;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection.Sorter;

public class EasiestFirstSorter : IBeatmapSorter
{
    public void Sort(List<BeatmapInfo> beatmapInfos)
    {
        beatmapInfos.Sort((a, b) => a.StarRating.CompareTo(b.StarRating));
    }
}
