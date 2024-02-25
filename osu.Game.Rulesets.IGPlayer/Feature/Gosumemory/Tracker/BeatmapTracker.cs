using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Logging;
using osu.Game.Beatmaps;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Tracker;

public partial class BeatmapTracker : AbstractTracker
{
    public BeatmapTracker(TrackerHub hub)
        : base(hub)
    {
    }

    private readonly Bindable<WorkingBeatmap> beatmap = new Bindable<WorkingBeatmap>();

    [BackgroundDependencyLoader]
    private void load(Bindable<WorkingBeatmap> globalBeatmap)
    {
        this.beatmap.BindTo(globalBeatmap);
    }

    protected override void LoadComplete()
    {
        Logger.Log("DDDLOADCOMPLETE");
        base.LoadComplete();

        this.beatmap.BindValueChanged(e =>
        {
            this.onBeatmapChanged(e.NewValue);
        }, true);
    }

    private void onBeatmapChanged(WorkingBeatmap newBeatmap)
    {
        Hub.GetDataRoot().UpdateBeatmap(newBeatmap);
    }
}
