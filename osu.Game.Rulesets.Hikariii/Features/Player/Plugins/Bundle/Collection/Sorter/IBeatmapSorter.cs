using System.Collections.Generic;
using osu.Game.Beatmaps;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection.Sorter;

public interface IBeatmapSorter
{
    public void Sort(List<BeatmapInfo> beatmapInfos);
}
