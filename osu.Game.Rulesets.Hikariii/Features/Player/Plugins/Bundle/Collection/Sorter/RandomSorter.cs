using System;
using System.Collections.Generic;
using osu.Game.Beatmaps;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection.Sorter;

public class RandomSorter : IBeatmapSorter
{
    public void Sort(List<BeatmapInfo> beatmapInfos)
    {
        var asArray = beatmapInfos.ToArray();
        var random = new Random();
        random.Shuffle(asArray);

        beatmapInfos.Clear();
        beatmapInfos.AddRange(asArray);
    }
}
