using osu.Framework.Allocation;
using osu.Game.Overlays;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.BuiltIn.OsuAudio;

public partial class DrawableOsuAudio : DrawableHikariiiPlugin, IProvideAudioControlPlugin
{
    [Resolved]
    private MusicController musicController { get; set; } = null!;

    public bool NextTrack()
    {
        musicController.NextTrack();
        return true;
    }

    public bool PrevTrack()
    {
        musicController.PreviousTrack();
        return true;
    }

    public bool TogglePause()
    {
        musicController.TogglePause();
        return true;
    }

    public bool Seek(double position)
    {
        return musicController.CurrentTrack.Seek(position);
    }

    public bool IsCurrent { get; set; }
    public bool AllowOsuControls => true;
}
