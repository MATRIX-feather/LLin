using System;
using System.Collections.Generic;
using osu.Game.Beatmaps;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection.Sorter;

public class RandomSorter : IBeatmapSorter
{
    private readonly Random random = new();

    public void Sort(List<BeatmapInfo> beatmapInfos)
    {
        var asArray = beatmapInfos.ToArray();
        random.Shuffle(asArray);

        beatmapInfos.Clear();
        beatmapInfos.AddRange(asArray);
    }

    public BeatmapInfo Pick(List<BeatmapInfo> beatmapInfos)
    {
        return beatmapInfos[random.Next(beatmapInfos.Count)];
    }
}
