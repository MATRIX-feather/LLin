using System.Collections.Generic;
using System.Linq;
using osu.Framework.Utils;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection.Utils;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection.Chooser;

public class RandomChooser(BeatmapManager beatmapManager) : IBeatmapChooser
{
    private readonly Queue<IBeatmapSetInfo> visitedSets = [];
    private readonly List<IBeatmapSetInfo> validSets = [];

    public void Activate(List<IBeatmapSetInfo> input)
    {
        ClearValidBeatmaps();
        this.validSets.AddRange(input);
    }

    public void Deactivate()
    {
        ClearValidBeatmaps();
    }

    public void ClearValidBeatmaps()
    {
        this.validSets.Clear();
        this.visitedSets.Clear();
    }

    public WorkingBeatmap? PickNext()
    {
        if (validSets.Count == 0)
            return null;

        var list = validSets.Except(visitedSets).ToList();

        if (list.Count == 0)
        {
            visitedSets.Clear();
            list.AddRange(validSets);
        }

        // Let's not make the queue too large
        if (visitedSets.Count > 100)
            visitedSets.Clear();

        var target = list[RNG.Next(0, list.Count)];
        visitedSets.Enqueue(target);

        return beatmapManager.GetWorkingBeatmap(target.Beatmaps.First().AsBeatmapInfo());
    }

    public WorkingBeatmap? PickLast()
    {
        if (validSets.Count == 0)
            return null;

        if (!visitedSets.TryDequeue(out var target))
            target = validSets[0];

        return beatmapManager.GetWorkingBeatmap(target.Beatmaps.First().AsBeatmapInfo());
    }

    public void OnExternalChoose(WorkingBeatmap beatmap)
    {
        var setInfo = beatmap.BeatmapSetInfo;

        if (validSets.Contains(setInfo))
            visitedSets.Enqueue(setInfo);
    }
}
