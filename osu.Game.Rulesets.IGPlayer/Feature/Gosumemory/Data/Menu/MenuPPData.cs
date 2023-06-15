using Newtonsoft.Json;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Data.Menu
{
    /// <summary>
    /// https://github.com/l3lackShark/gosumemory/blob/master/memory/values.go#L213
    /// </summary>
    public struct MenuPPData
    {
        [JsonProperty("100")]
        public int PPPerfect;

        [JsonProperty("99")]
        public int PP99;

        [JsonProperty("98")]
        public int PP98;

        [JsonProperty("97")]
        public int PP97;

        [JsonProperty("96")]
        public int PP96;

        [JsonProperty("95")]
        public int PP95;

        [JsonProperty("strains")]
        public float[] Strains;
    }
}
