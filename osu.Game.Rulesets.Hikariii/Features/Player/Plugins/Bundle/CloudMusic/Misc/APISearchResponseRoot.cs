using Newtonsoft.Json;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.CloudMusic.Misc
{
    public class APISearchResponseRoot
    {
        [JsonProperty("result")]
        public APISearchResultInfo? Result { get; set; }

        [JsonProperty("code")]
        public int ResponseCode { get; set; }
    }
}
