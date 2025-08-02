using System.Collections.Generic;
using osu.Game.Beatmaps;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection;

public interface IBeatmapChooser
{
    public void Activate(List<IBeatmapSetInfo> input);
    public void Deactivate();
    public void ClearValidBeatmaps();

    public WorkingBeatmap? PickNext();
    public WorkingBeatmap? PickLast();
    public void OnExternalChoose(WorkingBeatmap beatmap);
}
