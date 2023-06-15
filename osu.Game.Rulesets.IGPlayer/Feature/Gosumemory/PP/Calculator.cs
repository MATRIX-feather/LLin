using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Online.Spectator;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.PP;

public class Calculator
{
    private readonly Ruleset ruleset;

    private readonly PerformanceCalculator? performanceCalculator;

    public Calculator(Ruleset rulesetInstance, PerformanceCalculator performanceCalculator)
    {
        this.ruleset = rulesetInstance;
        this.performanceCalculator = performanceCalculator;
    }

    /// <summary>
    /// Please check https://github.com/ppy/osu/pull/22112
    /// </summary>
    /// <param name="score"></param>
    /// <param name="beatmap"></param>
    /// <returns></returns>
    public PerformanceAttributes CalculatePerfectPerformance(ScoreInfo score, IWorkingBeatmap beatmap)
    {
        if (performanceCalculator == null)
            return new PerformanceAttributes();

        ScoreInfo perfectPlay = score.DeepClone();
        IBeatmap playableBeatmap = beatmap.GetPlayableBeatmap(ruleset.RulesetInfo);
        perfectPlay.Accuracy = 1;
        perfectPlay.Passed = true;
        perfectPlay.MaxCombo = playableBeatmap.GetMaxCombo();

        // create statistics assuming all hit objects have perfect hit result
        var statistics = playableBeatmap.HitObjects
                                        .SelectMany(getPerfectHitResults)
                                        .GroupBy(hr => hr, (hr, list) => (hitResult: hr, count: list.Count()))
                                        .ToDictionary(pair => pair.hitResult, pair => pair.count);
        perfectPlay.Statistics = statistics;

        // calculate total score
        ScoreProcessor scoreProcessor = ruleset.CreateScoreProcessor();
        scoreProcessor.Mods.Value = perfectPlay.Mods;
        scoreProcessor.ApplyBeatmap(playableBeatmap);

        var frame = new ReplayFrame();
        var header = new FrameHeader(perfectPlay, scoreProcessor.GetScoreProcessorStatistics());
        frame.Header = header;

        scoreProcessor.ResetFromReplayFrame(frame);
        perfectPlay.TotalScore = scoreProcessor.TotalScore.Value;

        // compute rank achieved
        // default to SS, then adjust the rank with mods
        perfectPlay.Rank = ScoreRank.X;

        foreach (IApplicableToScoreProcessor mod in perfectPlay.Mods.OfType<IApplicableToScoreProcessor>())
        {
            perfectPlay.Rank = mod.AdjustRank(perfectPlay.Rank, 1);
        }

        DifficultyAttributes difficulty = ruleset.CreateDifficultyCalculator(beatmap).Calculate(score.Mods);

        return performanceCalculator.Calculate(perfectPlay, difficulty);
    }

    private IEnumerable<HitResult> getPerfectHitResults(HitObject hitObject)
    {
        foreach (HitObject nested in hitObject.NestedHitObjects)
            yield return nested.CreateJudgement().MaxResult;

        yield return hitObject.CreateJudgement().MaxResult;
    }
}
