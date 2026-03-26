using System.Collections.Generic;
using osu.Game.Beatmaps;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection.Sorter;

public class NoOpSorter : IBeatmapSorter
{
    public void Sort(List<BeatmapInfo> beatmapInfos)
    {
    }

    public BeatmapInfo Pick(List<BeatmapInfo> beatmapInfos)
    {
        return beatmapInfos[0];
    }
}
