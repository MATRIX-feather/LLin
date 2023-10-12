using osu.Framework.Audio.Track;
using osu.Framework.Timing;

namespace osu.Game.Rulesets.IGPlayer.Player.Misc;

public class AudioClock : InterpolatingFramedClock
{
    public Track bindingTrack;
}
