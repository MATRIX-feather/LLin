using Newtonsoft.Json;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.LyricProvider.Netease.Response
{
    public class APISongInfo
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }
    }
}
