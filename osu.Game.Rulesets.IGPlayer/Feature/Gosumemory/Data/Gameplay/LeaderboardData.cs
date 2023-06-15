using Newtonsoft.Json;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Data.Gameplay
{
    public struct LeaderboardData
    {
        [JsonProperty("hasLeaderboard")]
        public bool HasLeaderBoard;

        [JsonProperty("isVisible")]
        public bool IsVisible;

        [JsonProperty("ourPlayer")]
        public LeaderboardPlayer? OurPlayer;

        [JsonProperty("slots")]
        public LeaderboardPlayer[]? Slots;
    }

    public struct LeaderboardPlayer
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("score")]
        public int Score;

        [JsonProperty("combo")]
        public int Combo;

        [JsonProperty("maxCombo")]
        public int MaxCombo;

        [JsonProperty("mods")]
        public string Mods;

        [JsonProperty("h300")]
        public int Hit300;

        [JsonProperty("h100")]
        public int Hit100;

        [JsonProperty("h50")]
        public int Hit50;

        [JsonProperty("h0")]
        public int HitMiss;

        [JsonProperty("team")]
        public int Team;

        [JsonProperty("position")]
        public int Position;

        [JsonProperty("isPassing")]
        public int IsPassing;
    }
}
