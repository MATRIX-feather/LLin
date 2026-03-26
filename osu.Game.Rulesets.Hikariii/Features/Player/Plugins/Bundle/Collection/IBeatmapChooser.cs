using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection.Sorter;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection;

public interface IBeatmapChooser
{
    public void SetBeatmapCandidates(Dictionary<BeatmapSetInfo, List<BeatmapInfo>> beatmapDictionary);
    public void ClearBeatmapCandidates();
    public void SetCandidateSortMethod(SortMethod? sortMethod);

    public WorkingBeatmap? PickNext();
    public WorkingBeatmap? PickLast();
    public WorkingBeatmap? PickFrom(BeatmapSetInfo beatmapSet);
    public void OnExternalChoose(WorkingBeatmap beatmap);
}
