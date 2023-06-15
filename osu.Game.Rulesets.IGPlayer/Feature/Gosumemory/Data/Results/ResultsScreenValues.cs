using Newtonsoft.Json;
using osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Data.Menu;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Data.Results
{
    public struct ResultsScreenValues
    {
        [JsonProperty("name")]
        public string? Name;

        [JsonProperty("score")]
        public int Score;

        [JsonProperty("maxCombo")]
        public int MaxCombo;

        [JsonProperty("mods")]
        public MenuModsData Mods;

        [JsonProperty("300")]
        public int Score300Count;

        [JsonProperty("geki")]
        public int ScoreGekiCount;

        [JsonProperty("100")]
        public int Score100Count;

        [JsonProperty("katu")]
        public int ScoreKatuCount;

        [JsonProperty("50")]
        public int Score50Count;

        [JsonProperty("0")]
        public int ScoreMissCount;
    }
}
