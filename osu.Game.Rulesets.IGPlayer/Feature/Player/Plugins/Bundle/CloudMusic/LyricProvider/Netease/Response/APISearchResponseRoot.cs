using Newtonsoft.Json;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.LyricProvider.Netease.Response
{
    public class APISearchResponseRoot
    {
        [JsonProperty("result")]
        public APISearchResultInfo? Result { get; set; }

        [JsonProperty("code")]
        public int ResponseCode { get; set; }
    }
}
