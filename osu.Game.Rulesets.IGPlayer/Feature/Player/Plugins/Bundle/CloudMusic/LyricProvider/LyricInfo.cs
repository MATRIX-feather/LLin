using System.Collections.Generic;
using Newtonsoft.Json;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.Misc;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.LyricProvider;

public class LyricMeta
{
    [JsonProperty("lrc")]
    public List<Lyric> Lyrics;

    [JsonProperty("offset")]
    public double Offset;
}
