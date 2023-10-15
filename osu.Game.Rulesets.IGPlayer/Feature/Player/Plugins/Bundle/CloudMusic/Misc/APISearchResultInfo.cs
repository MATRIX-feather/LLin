using System.Collections.Generic;
using Newtonsoft.Json;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.Misc
{
    public class APISearchResultInfo
    {
        [JsonProperty("songs")]
        public IList<APISongInfo>? Songs { get; set; }

        [JsonProperty("songCount")]
        public int SongCount { get; set; }
    }
}
