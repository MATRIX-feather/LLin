using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.Misc
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class APIArtistInfo
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PicUrl { get; set; } = string.Empty;
        public IList<string>? Alias { get; set; }
        public int AlbunSize { get; set; }
        public int PicId { get; set; }

        [JsonProperty("img1v1Url")]
        public string Img1V1Url { get; set; } = string.Empty;

        [JsonProperty("img1v1")]
        public int Img1V1 { get; set; }

        public string? Trans { get; set; }
        public int AlbumSize { get; set; }
    }
}
