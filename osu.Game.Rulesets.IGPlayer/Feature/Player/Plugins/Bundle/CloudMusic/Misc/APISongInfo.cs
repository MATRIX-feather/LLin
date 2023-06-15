using Newtonsoft.Json;

namespace osu.Game.Rulesets.IGPlayer.Player.Plugins.Bundle.CloudMusic.Misc
{
    public class APISongInfo
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }
    }
}
