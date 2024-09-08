using Newtonsoft.Json;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.LyricProvider.Netease.Response
{
    public class LyricInfo
    {
        [JsonProperty("lyric")]
        public string? RawLyric { get; set; }

        [JsonProperty("version")]
        public int Version;
    }
}
