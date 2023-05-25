using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Drawables;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online;
using osu.Game.Rulesets.IGPlayer.Player.DownloadAccel;
using osu.Game.Rulesets.IGPlayer.Player.Injectors;

namespace osu.Game.Rulesets.IGPlayer.Player.Graphics;

public partial class AccelDownloadProgressBar : DownloadProgressBar
{
    public AccelDownloadProgressBar(IBeatmapSetInfo beatmapSet)
        : base(beatmapSet)
    {
        this.beatmapSetInfo = beatmapSet;
    }

    private readonly IBeatmapSetInfo beatmapSetInfo;

    [BackgroundDependencyLoader]
    private void load()
    {
        var accelTracker = new AccelBeatmapDownloadTracker(beatmapSetInfo);
        AddInternal(accelTracker);

        if (this.FindInstance(typeof(ProgressBar)) is not ProgressBar progressBar) return;

        progressBar.Current.UnbindBindings();
        progressBar.Current.BindTarget = accelTracker.Progress;

        var dlTracker = this.FindInstance(typeof(BeatmapDownloadTracker)) as BeatmapDownloadTracker;
        ((Bindable<DownloadState>)dlTracker.State).BindTarget = accelTracker.State;
        ((BindableNumber<double>)dlTracker.Progress).BindTarget = accelTracker.Progress;
    }
}
