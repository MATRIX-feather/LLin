using Newtonsoft.Json;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Data.Common
{
    public struct HitCounts
    {
        /// <summary>
        /// 300
        /// </summary>
        [JsonProperty("300")]
        public int Perfect;

        /// <summary>
        /// 似乎Lazer里没有Geki、Katu的概念（至少我没在结算的时候看到过
        /// </summary>
        [JsonProperty("geki")]
        public int Geki;

        /// <summary>
        /// 100
        /// </summary>
        [JsonProperty("100")]
        public int Great;

        /// <summary>
        /// Katu
        /// </summary>
        [JsonProperty("katu")]
        public int Katu;

        [JsonProperty("50")]
        public int Meh;

        [JsonProperty("0")]
        public int Miss;

        [JsonProperty("sliderBreaks")]
        public int SliderBreaks;

        [JsonProperty("grade")]
        public GradeInfo Grade;

        [JsonProperty("unstableRate")]
        public float UnstableRate;

        [JsonProperty("hitErrorArray")]
        public int[] HitErrors;
    }
}
