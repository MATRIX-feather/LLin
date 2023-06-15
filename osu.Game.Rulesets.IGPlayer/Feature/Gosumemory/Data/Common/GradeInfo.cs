using Newtonsoft.Json;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Data.Common
{
    public struct GradeInfo
    {
        [JsonProperty("current")]
        public string Current;

        [JsonProperty("maxThisPlay")]
        public string Expected;
    }
}
