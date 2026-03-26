using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection.Chooser;

public class SequenceChooser(BeatmapManager beatmapManager) : IBeatmapChooser
{
    private readonly List<BeatmapInfo> currentList = [];
    private int currentIndex = 0;

    public void Activate(ICollection<BeatmapInfo> input)
    {
        ClearValidBeatmaps();
        currentList.AddRange(input);
    }

    public void Deactivate()
    {
        ClearValidBeatmaps();
    }

    public void ClearValidBeatmaps()
    {
        currentIndex = -1;
        currentList.Clear();
    }

    public WorkingBeatmap? PickNext()
    {
        if (currentList.Count == 0)
            return null;

        currentIndex++;

        if (currentIndex >= currentList.Count)
            currentIndex = 0;

        return beatmapManager.GetWorkingBeatmap(currentList[currentIndex]);
    }

    public WorkingBeatmap? PickLast()
    {
        if (currentList.Count == 0)
            return null;

        currentIndex--;

        if (currentIndex < 0)
            currentIndex = currentList.Count - 1;

        return beatmapManager.GetWorkingBeatmap(currentList[currentIndex]);
    }

    public void OnExternalChoose(WorkingBeatmap beatmap)
    {
        var beatmapSetInfo = beatmap.BeatmapSetInfo;
        var match = currentList.FirstOrDefault(i => i.BeatmapSet?.Equals(beatmapSetInfo) ?? false);

        int index = match == null
            ? -1
            : currentList.IndexOf(match);

        currentIndex = index;
    }
}
