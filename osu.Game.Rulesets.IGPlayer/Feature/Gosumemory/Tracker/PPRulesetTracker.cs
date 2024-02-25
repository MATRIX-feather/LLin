using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Data.Consts;
using osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.PP;
using osu.Game.Rulesets.Mods;
using osu.Game.Scoring;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Tracker;

/// <summary>
/// 用于更新一些游戏外的PP/游戏模式信息
/// </summary>
public partial class PPRulesetTracker : AbstractTracker
{
    public PPRulesetTracker(TrackerHub hub)
        : base(hub)
    {
    }

    [Resolved]
    private Bindable<RulesetInfo> ruleset { get; set; } = null!;

    [Resolved]
    private IBindable<IReadOnlyList<Mod>> mods { get; set; } = null!;

    private Ruleset? rsInstance;
    private PerformanceCalculator? performanceCalculator;

    private readonly Bindable<WorkingBeatmap> working = new Bindable<WorkingBeatmap>();

    private CancellationTokenSource? ppCalcTokenSource;

    [BackgroundDependencyLoader]
    private void load(Bindable<WorkingBeatmap> globalWorking)
    {
        this.working.BindTo(globalWorking);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        // Mods产生变动时视为谱面更新并重新计算PP
        mods.BindValueChanged(e =>
        {
            var newMods = e.NewValue;
            var dataRoot = Hub.GetDataRoot();

            dataRoot.MenuValues.Mods.AppliedMods = newMods.Count;

            if (newMods.Count >= 1)
            {
                string str = newMods.Aggregate("", (current, mod) => current + $"{mod.Acronym}");
                dataRoot.MenuValues.Mods.Acronyms = str;
            }
            else
            {
                dataRoot.MenuValues.Mods.Acronyms = "NM";
            }

            working.TriggerChange();
        }, true);

        // 游戏模式产生变动时更新相关信息并视为谱面更新重新计算PP
        ruleset.BindValueChanged(v =>
        {
            this.rsInstance = v.NewValue.CreateInstance();
            this.performanceCalculator = rsInstance.CreatePerformanceCalculator();
            Hub.GetDataRoot().GameplayValues.Gamemode = LegacyGamemodes.FromRulesetInfo(v.NewValue);

            working.TriggerChange();
        }, true);

        // 谱面产生变动时更新PP信息
        working.BindValueChanged(e =>
        {
            var modsCopy = mods.Value.Where(m => m.Acronym != "CL").Select(m => m.DeepClone()).ToArray();

            runCalculateMaxPP(e.NewValue, modsCopy)
                .ContinueWith(task =>
                {
                    if (!task.IsCompleted) return;

                    this.Schedule(() =>
                    {
                        var result = task.GetResultSafely();

                        var dataRoot = Hub.GetDataRoot();
                        dataRoot.GameplayValues.pp.MaxThisPlay = dataRoot.GameplayValues.pp.PPIfFc = result.MaxPP;
                        dataRoot.MenuValues.pp.PPPerfect = result.MaxPP;
                    });
                });
        }, true);
    }

    private struct PerformanceInfo
    {
        public int MaxPP;
    }

    private Task<PerformanceInfo> runCalculateMaxPP(WorkingBeatmap workingBeatmap, Mod[] modsCopy)
    {
        ppCalcTokenSource?.Cancel();
        ppCalcTokenSource = new CancellationTokenSource();

        return Task.Run(async () => await calculateMaxmiumPerformancePoints(workingBeatmap, modsCopy).ConfigureAwait(false), ppCalcTokenSource.Token);
    }

    private Task<PerformanceInfo> calculateMaxmiumPerformancePoints(WorkingBeatmap workingBeatmap, Mod[] modsCopy)
    {
        int maxpp = 0;

        try
        {
            var score = new ScoreInfo(workingBeatmap.BeatmapInfo, ruleset.Value)
            {
                Mods = modsCopy
            };

            PerformanceAttributes performanceAttribute;

            if (!rsInstance?.CreateBeatmapConverter(workingBeatmap.Beatmap).CanConvert() ?? true)
                performanceAttribute = new PerformanceAttributes();
            else
            {
                // 使用此rsInstance的performanceCalc
                performanceAttribute = this.performanceCalculator != null
                    ? new Calculator(rsInstance, this.performanceCalculator).CalculatePerfectPerformance(score, workingBeatmap)
                    : new PerformanceAttributes();
            }

            maxpp = (int)performanceAttribute.Total;
        }
        catch (Exception e)
        {
            Logging.LogError(e, "Error occurred while calculating performance point!");
        }

        var info = new PerformanceInfo
        {
            MaxPP = maxpp
        };

        return Task.FromResult(info);
    }
}
