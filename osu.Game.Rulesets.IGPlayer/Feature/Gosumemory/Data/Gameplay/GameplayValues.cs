using System.Collections.Generic;
using Newtonsoft.Json;
using osu.Framework.Extensions;
using osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Data.Common;
using osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Extensions;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Data.Gameplay
{
    public struct GameplayValues
    {
        [JsonProperty("gameMode")]
        public int Gamemode;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("score")]
        public int Score;

        [JsonProperty("accuracy")]
        public float Accuracy;

        [JsonProperty("combo")]
        public ComboInfo ComboInfo;

        [JsonProperty("hp")]
        public HealthInfo Hp;

        [JsonProperty("hits")]
        public HitCounts HitResults;

        [JsonProperty("pp")]
        public GameplayPPData pp;

        [JsonProperty("leaderboard")]
        public LeaderboardData Leaderboard;

        [JsonProperty("keyOverlay")]
        public KeyOverlayData KeyOverlay;

        public void FromScore(ScoreInfo scoreInfo)
        {
            ComboInfo.MaxCombo = scoreInfo.MaxCombo;
            ComboInfo.CurrentCombo = scoreInfo.Combo;

            var stat = scoreInfo.Statistics;

            switch (scoreInfo.Ruleset.Name)
            {
                default:
                case "osu!":
                    HitResults.Perfect = scoreInfo.GetResultsPerfect();
                    HitResults.Great = scoreInfo.GetResultsGreat();
                    HitResults.Meh = stat.GetValueOrDefault(HitResult.Meh, 0);
                    HitResults.Miss = stat.GetValueOrDefault(HitResult.Miss, 0);
                    break;

                case "osu!mania":
                    HitResults.Geki = stat.GetValueOrDefault(HitResult.Perfect, 0);
                    HitResults.Perfect = stat.GetValueOrDefault(HitResult.Great, 0);
                    HitResults.Great = stat.GetValueOrDefault(HitResult.Good, 0);
                    HitResults.Katu = stat.GetValueOrDefault(HitResult.Ok, 0);

                    HitResults.Meh = stat.GetValueOrDefault(HitResult.Meh, 0);
                    HitResults.Miss = stat.GetValueOrDefault(HitResult.Miss, 0);
                    break;
            }

            Score = (int)scoreInfo.TotalScore;
            Accuracy = 100 * (float)scoreInfo.Accuracy;
            HitResults.Grade.Current = scoreInfo.Rank.GetDescription().Replace("+", "");

            HitResults.UnstableRate = (float?)scoreInfo.HitEvents.CalculateUnstableRate() ?? 0;
        }

        /// <summary>
        /// 重置此<see cref="GameplayValues"/>除Gamemode和Name以外的所有值
        /// </summary>
        public void Reset()
        {
            ComboInfo.MaxCombo = ComboInfo.CurrentCombo = 0;
            Hp.Normal = Hp.Smooth = 0f;

            HitResults.Perfect = 0;
            HitResults.Great = 0;
            HitResults.Meh = 0;
            HitResults.Miss = 0;

            Leaderboard.Slots = new LeaderboardPlayer[] { };
            Leaderboard.IsVisible = false;
            Leaderboard.OurPlayer = null;
            Leaderboard.HasLeaderBoard = false;

            KeyOverlay.Reset();

            Score = 0;
            Accuracy = 0;
        }
    }
}
