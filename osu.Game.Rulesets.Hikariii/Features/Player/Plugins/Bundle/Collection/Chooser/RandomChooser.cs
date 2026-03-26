using System.Collections.Generic;
using System.Linq;
using osu.Framework.Utils;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection.Sorter;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection.Chooser;

public class RandomChooser(BeatmapManager beatmapManager) : IBeatmapChooser
{
    private readonly Dictionary<BeatmapSetInfo, List<BeatmapInfo>> beatmapCandidates = [];
    private readonly List<BeatmapSetInfo> visitedBeatmapSets = [];

    private void sortBeatmaps(SortMethod sortMethod)
    {
        foreach (var keyValuePair in beatmapCandidates)
            sorter.Sort(keyValuePair.Value);
    }

    public void SetBeatmapCandidates(Dictionary<BeatmapSetInfo, List<BeatmapInfo>> beatmapDictionary)
    {
        ClearBeatmapCandidates();

        foreach (var keyValuePair in beatmapDictionary)
            beatmapCandidates[keyValuePair.Key] = keyValuePair.Value;

        sortBeatmaps(sortMethod);
    }

    public void ClearBeatmapCandidates()
    {
        beatmapCandidates.Clear();
        visitedBeatmapSets.Clear();
    }

    private SortMethod sortMethod = SortMethod.MostDifficultFirst;
    private IBeatmapSorter sorter;

    public void SetCandidateSortMethod(SortMethod? newMethod)
    {
        this.sortMethod = newMethod ?? SortMethod.MostDifficultFirst;

        this.sorter = sortMethod switch
        {
            SortMethod.MostDifficultFirst => new MostDifficultFirstSorter(),
            SortMethod.EasiestFirst => new EasiestFirstSorter(),
            SortMethod.Random => new RandomSorter(),
            _ => new NoOpSorter()
        };

        sortBeatmaps(this.sortMethod);
    }

    public WorkingBeatmap? PickNext()
    {
        if (beatmapCandidates.Count == 0)
            return null;

        var visitableBeatmapSets = beatmapCandidates.Keys.Except(visitedBeatmapSets).ToList();

        if (visitableBeatmapSets.Count == 0)
        {
            visitedBeatmapSets.Clear();
            visitableBeatmapSets.AddRange(beatmapCandidates.Keys);
        }

        // Let's not make the queue too large
        if (visitedBeatmapSets.Count > 100)
            visitedBeatmapSets.Clear();

        var targetBeatmapSet = visitableBeatmapSets[RNG.Next(0, visitableBeatmapSets.Count)];
        visitedBeatmapSets.Add(targetBeatmapSet);

        return beatmapManager.GetWorkingBeatmap(sorter.Pick(beatmapCandidates[targetBeatmapSet]));
    }

    public WorkingBeatmap? PickLast()
    {
        if (beatmapCandidates.Count == 0)
            return null;

        BeatmapSetInfo targetBeatmapSet;

        if (visitedBeatmapSets.Count == 0)
        {
            targetBeatmapSet = beatmapCandidates.Keys.First();
        }
        else
        {
            targetBeatmapSet = visitedBeatmapSets[^1];
            visitedBeatmapSets.Remove(targetBeatmapSet);
        }

        return beatmapManager.GetWorkingBeatmap(sorter.Pick(beatmapCandidates[targetBeatmapSet]));
    }

    public WorkingBeatmap? PickFrom(BeatmapSetInfo beatmapSet)
    {
        return !beatmapCandidates.TryGetValue(beatmapSet, out var list)
            ? null
            : beatmapManager.GetWorkingBeatmap(sorter.Pick(list));
    }

    public void OnExternalChoose(WorkingBeatmap beatmap)
    {
        var setInfo = beatmap.BeatmapSetInfo;

        if (beatmapCandidates.ContainsKey(setInfo))
            visitedBeatmapSets.Add(setInfo);
    }
}
