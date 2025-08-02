using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection.Utils;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection.Chooser;

public class SequenceChooser(BeatmapManager beatmapManager) : IBeatmapChooser
{
    private readonly List<IBeatmapSetInfo> currentList = [];
    private int currentIndex = 0;

    public void Activate(List<IBeatmapSetInfo> input)
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

        return beatmapManager.GetWorkingBeatmap(currentList[currentIndex].Beatmaps.First().AsBeatmapInfo());
    }

    public WorkingBeatmap? PickLast()
    {
        if (currentList.Count == 0)
            return null;

        currentIndex--;

        if (currentIndex < 0)
            currentIndex = currentList.Count - 1;

        return beatmapManager.GetWorkingBeatmap(currentList[currentIndex].Beatmaps.First().AsBeatmapInfo());
    }

    public void OnExternalChoose(WorkingBeatmap beatmap)
    {
        currentIndex = currentList.Contains(beatmap.BeatmapSetInfo)
            ? currentList.IndexOf(beatmap.BeatmapSetInfo)
            : -1;
    }
}
