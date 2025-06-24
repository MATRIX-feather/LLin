using osu.Framework.Allocation;
using osu.Game.Overlays;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces;
using osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Media.Source;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Screens.LLin;

public partial class LLinMediaSource : OsuMediaSource
{
    private readonly IImplementLLin llin;

    public LLinMediaSource(IImplementLLin screen)
    {
        this.llin = screen;
    }

    [Resolved]
    private MusicController musicController { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        llin.OnTrackRunningToggle += running => OnPlayPauseUpdate?.Invoke(!running);
    }

    public override void OnExternalNext()
    {
        llin.Next();
    }

    public override void OnExternalPrevious()
    {
        llin.Previous();
    }
}
