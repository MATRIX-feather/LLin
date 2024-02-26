using Newtonsoft.Json;
using osu.Framework.Graphics.Audio;
using osu.Game.Beatmaps;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Data.Menu
{
    public struct MainMenuValues
    {
        [JsonProperty("bassDensity")]
        public float BassDensity;
    }

    public struct MenuValues : ISelfUpdatableFromBeatmap, ISelfUpdatableFromAudio
    {
        [JsonProperty("mainMenu")]
        public MainMenuValues MainMenuValues;

        [JsonProperty("state")]
        public int OsuState;

        [JsonProperty("gameMode")]
        public int GameMode;

        [JsonProperty("isChatEnabled")]
        public bool ChatEnabled;

        [JsonProperty("bm")]
        public GosuBeatmapInfo GosuBeatmapInfo;

        [JsonProperty("mods")]
        public MenuModsData Mods;

        [JsonProperty("pp")]
        public MenuPPData pp;

        public void UpdateTrack(DrawableTrack track)
        {
            this.GosuBeatmapInfo.UpdateTrack(track);
        }

        public void UpdateMetadata(WorkingBeatmap beatmap)
        {
            this.GosuBeatmapInfo.UpdateMetadata(beatmap);
        }
    }
}
