using System.Collections.Generic;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Extensions
{
    public static class ScoreInfoExtension
    {
        public static int GetResultsPerfect(this ScoreInfo scoreInfo)
        {
            return scoreInfo.Statistics.GetValueOrDefault(HitResult.Perfect, 0) + scoreInfo.Statistics.GetValueOrDefault(HitResult.Great, 0);
        }

        public static int GetResultsGreat(this ScoreInfo scoreInfo)
        {
            return scoreInfo.Statistics.GetValueOrDefault(HitResult.Good, 0) + scoreInfo.Statistics.GetValueOrDefault(HitResult.Ok, 0);
        }

        public static int GetAllMisses(this ScoreInfo scoreInfo)
        {
            var stat = scoreInfo.Statistics;

            return stat.GetValueOrDefault(HitResult.SmallTickMiss, 0)
                   + stat.GetValueOrDefault(HitResult.LargeTickMiss, 0)
                   + stat.GetValueOrDefault(HitResult.Miss, 0);
        }
    }
}
