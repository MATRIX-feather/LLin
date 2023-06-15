using System;
using Newtonsoft.Json;
using osu.Framework.Graphics.Audio;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Data.Gameplay;
using osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Data.Menu;
using osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Data.Results;
using osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Data.Settings;
using osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Data.Tourney;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Data
{
    public class DataRoot : ISelfUpdatableFromBeatmap, ISelfUpdatableFromAudio
    {
        [JsonProperty("settings")]
        public SettingsValues SettingsValues;

        [JsonProperty("menu")]
        public MenuValues MenuValues;

        [JsonProperty("gameplay")]
        public GameplayValues GameplayValues;

        [JsonProperty("resultsScreen")]
        public ResultsScreenValues ResultsScreenValues;

        [Obsolete("Not planned")]
        [JsonProperty("tourney")]
        public TourneyValues TourneyValues = new TourneyValues();

        public void UpdateBeatmap(WorkingBeatmap workingBeatmap)
        {
            MenuValues.UpdateBeatmap(workingBeatmap);
        }

        public void UpdateTrack(DrawableTrack track)
        {
            MenuValues.UpdateTrack(track);
        }
    }
}
