using Newtonsoft.Json;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Data.Menu
{
    public struct MenuModsData
    {
        [JsonProperty("num")]
        public int AppliedMods;

        [JsonProperty("str")]
        public string Acronyms;
    }
}
