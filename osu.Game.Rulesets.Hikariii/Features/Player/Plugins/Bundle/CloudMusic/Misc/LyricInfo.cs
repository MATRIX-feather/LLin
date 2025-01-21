using Newtonsoft.Json;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.CloudMusic.Misc
{
    public class LyricInfo
    {
        [JsonProperty("lyric")]
        public string? RawLyric { get; set; }

        [JsonProperty("version")]
        public int Version;
    }
}
