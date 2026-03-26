using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection.Sorter;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection.Chooser;

public class SequenceChooser(BeatmapManager beatmapManager) : IBeatmapChooser
{
    private readonly Dictionary<BeatmapSetInfo, List<BeatmapInfo>> beatmapCandidates = [];
    private int currentIndex = 0;

    private IBeatmapSorter sorter = new NoOpSorter();

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
        currentIndex = -1;
        beatmapCandidates.Clear();
    }

    private SortMethod sortMethod = SortMethod.MostDifficultFirst;

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

        currentIndex++;

        if (currentIndex >= beatmapCandidates.Count)
            currentIndex = 0;

        return pickAt(currentIndex);
    }

    public WorkingBeatmap? PickLast()
    {
        if (beatmapCandidates.Count == 0)
            return null;

        currentIndex--;

        if (currentIndex < 0)
            currentIndex = beatmapCandidates.Count - 1;

        return pickAt(currentIndex);
    }

    private WorkingBeatmap? pickAt(int index)
    {
        var enumerator = beatmapCandidates.Keys.GetEnumerator();

        for (int i = 0; i <= index; i++)
        {
            if (!enumerator.MoveNext())
                return null;
        }

        return beatmapManager.GetWorkingBeatmap(sorter.Pick(beatmapCandidates[enumerator.Current]));
    }

    public WorkingBeatmap? PickFrom(BeatmapSetInfo beatmapSet)
    {
        return !beatmapCandidates.TryGetValue(beatmapSet, out var list)
            ? null
            : beatmapManager.GetWorkingBeatmap(sorter.Pick(list));
    }

    public void OnExternalChoose(WorkingBeatmap beatmap)
    {
        var beatmapSetInfo = beatmap.BeatmapSetInfo;

        if (!beatmapCandidates.ContainsKey(beatmapSetInfo))
        {
            currentIndex = -1;
            return;
        }

        var enumerator = beatmapCandidates.Keys.GetEnumerator();

        for (int i = 0; i < beatmapCandidates.Count; i++)
        {
            if (!enumerator.MoveNext())
                break;

            var current = enumerator.Current;

            if (!current.Equals(beatmapSetInfo)) continue;

            currentIndex = i;
            break;
        }
    }
}
