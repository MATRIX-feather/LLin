using System;
using Newtonsoft.Json;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Data.Settings
{
    public struct SettingsValues
    {
        [JsonProperty("showInterface")]
        public bool ShowInterface;

        [Obsolete("Not planned")]
        [JsonProperty("folders")]
        public object[]? Folders;
    }
}
