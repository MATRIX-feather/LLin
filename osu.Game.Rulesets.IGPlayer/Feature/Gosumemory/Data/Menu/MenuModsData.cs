using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Data.Menu
{
    public struct MenuModsData
    {
        [JsonProperty("num")]
        public int AppliedMods;

        [JsonProperty("str")]
        public string Acronyms;

        public void UpdateFrom(IReadOnlyList<Mod> mods)
        {
            this.AppliedMods = mods.Count;

            if (mods.Count >= 1)
            {
                string str = mods.Aggregate("", (current, mod) => current + $"{mod.Acronym}");
                this.Acronyms = str;
            }
            else
                this.Acronyms = "NM";
        }
    }
}
