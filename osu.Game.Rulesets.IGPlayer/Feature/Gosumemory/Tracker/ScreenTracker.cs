using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osu.Game.Beatmaps;
using osu.Game.Online.API;
using osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Data.Consts;
using osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Data.Gameplay;
using osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Extensions;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;
using osu.Game.Screens;
using osu.Game.Screens.Edit;
using osu.Game.Screens.Play;
using osu.Game.Screens.Play.HUD;
using osu.Game.Screens.Ranking;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Tracker;

public partial class ScreenTracker : AbstractTracker
{
    public ScreenTracker(TrackerHub hub)
        : base(hub)
    {
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        LocateScreenStack();

        if (screenStack == null && !warningPrinted)
        {
            Logging.Log("无法定位到OsuScreenStack, 一些功能可能不会正常运作", level: LogLevel.Important);
            warningPrinted = true;
        }
    }

    [Resolved]
    private OsuGame game { get; set; } = null!;

    [Resolved]
    private IAPIProvider api { get; set; } = null!;

    private bool warningPrinted;
    private OsuScreenStack? screenStack;

    protected OsuScreenStack? getScreenStack()
    {
        return screenStack;
    }

    public void LocateScreenStack()
    {
        if (screenStack != null) return;

        const BindingFlags flag = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic;
        var screenStackField = game.GetType().GetFields(flag).FirstOrDefault(f => f.FieldType == typeof(OsuScreenStack));

        if (screenStackField == null) return;

        object? val = screenStackField.GetValue(game);

        if (val is not OsuScreenStack osuScreenStack) return;

        screenStack = osuScreenStack;

        screenStack.ScreenExited += onScreenSwitch;
        screenStack.ScreenPushed += onScreenSwitch;
    }

    private Screens.Play.Player? playerScreen;

    private ResultsScreen? resultsScreen;

    private CancellationTokenSource? scorePPCalcTokenSource;

    private string playerName = "???";

    private HealthProcessorAccessor? healthProcessorAccessor;

    private bool isInGame()
    {
        return playerScreen != null;
    }

    private CounterContainer? inGamePPCounter;

    private int performanceThisRun = 0;

    public override void UpdateValues()
    {
        base.UpdateValues();

        if (!isInGame())
        {
            if (inGamePPCounter == null) return;

            inGamePPCounter.Expire();
            inGamePPCounter = null;

            return;
        }

        var scoreInfo = playerScreen!.Score?.ScoreInfo.DeepClone();

        //scoreInfo in EndlessPlayer would be null for somehow
        if (scoreInfo == null)
            return;

        playerScreen!.GameplayState.ScoreProcessor.PopulateScore(scoreInfo);

        var dataRoot = Hub.GetDataRoot();
        dataRoot.GameplayValues.FromScore(scoreInfo);

        if (inGamePPCounter == null)
            AddInternal(inGamePPCounter = new CounterContainer(playerScreen.GameplayState, playerScreen.GameplayState.ScoreProcessor));

        double health = 200 * (healthProcessorAccessor?.HealthProcessor.Health.Value ?? 0d);
        dataRoot.GameplayValues.Hp.Smooth = dataRoot.GameplayValues.Hp.Normal;
        dataRoot.GameplayValues.Hp.Normal = (float)health;

        performanceThisRun = inGamePPCounter.Current.Value;
        dataRoot.GameplayValues.pp.Current = this.performanceThisRun;
    }

    private void onScreenSwitch(IScreen prevScreen, IScreen nextScreen)
    {
        scorePPCalcTokenSource?.Cancel();

        this.playerScreen = null;
        this.resultsScreen = null;

        //updateCancellationTokenSource?.Cancel();

        var dataRoot = Hub.GetDataRoot();

        //从结算切换到其他页面：重置游玩数据
        if (prevScreen is ResultsScreen || nextScreen is PlayerLoader)
        {
            performanceThisRun = 0;
            dataRoot.GameplayValues.Reset();
        }

        healthProcessorAccessor?.Expire();
        healthProcessorAccessor = null;

        switch (nextScreen)
        {
            case ResultsScreen results:
                this.onResultsScreen(results);
                break;

            case Screens.Play.Player player:
                this.onPlayerScreen(player);
                break;

            case Editor:
                dataRoot.MenuValues.OsuState = OsuStates.EDITOR;
                break;

            default:
                dataRoot.MenuValues.OsuState = OsuStates.IDLE;
                break;
        }

        dataRoot.GameplayValues.Name = playerName = (playerScreen != null)
            ? (playerScreen.Score?.ScoreInfo?.User.Username ?? "???")
            : (resultsScreen != null ? (resultsScreen.SelectedScore.Value?.User.Username ?? "???") : api.LocalUser.Value.Username);
    }

    [Resolved]
    private BeatmapDifficultyCache beatmapDifficultyCache { get; set; } = null!;

    private void onResultsScreen(ResultsScreen results)
    {
        var score = results.SelectedScore.Value;
        var dataRoot = Hub.GetDataRoot();

        dataRoot.MenuValues.OsuState = OsuStates.IDLE;
        this.resultsScreen = results;
        Hub.GetDataRoot().MenuValues.OsuState = OsuStates.PLAYING;

        if (score == null) return;

        dataRoot.GameplayValues.FromScore(score);
        dataRoot.GameplayValues.pp.Current = (int?)score.PP ?? 0;

        if (score.PP.HasValue)
        {
            dataRoot.GameplayValues.pp.Current = (int)score.PP;
        }
        else
        {
            scorePPCalcTokenSource?.Cancel();
            scorePPCalcTokenSource = new CancellationTokenSource();

            if (score.BeatmapInfo != null)
            {
                // 参考了 osu.Game.Screens.Ranking.Expanded.Statistics.PerformanceStatistic
                Task.Run(async () =>
                {
                    double pp = await calculatePPFromScore(score, scorePPCalcTokenSource.Token).ConfigureAwait(false);
                    this.Schedule(() => dataRoot.GameplayValues.pp.Current = (int)Math.Floor(pp));
                }, scorePPCalcTokenSource.Token);
            }
            else
            {
                Logging.Log("score.BeatmapInfo is null?! Not updating pp to gosu...");
            }
        }
    }

    private async Task<double> calculatePPFromScore(ScoreInfo score, CancellationToken cancellationToken)
    {
        if (score.BeatmapInfo == null) return 0d;

        var scorePPCalculator = score.Ruleset.CreateInstance().CreatePerformanceCalculator();
        var starDiff = await beatmapDifficultyCache.GetDifficultyAsync(score.BeatmapInfo, score.Ruleset, score.Mods, cancellationToken).ConfigureAwait(false);

        if (starDiff?.Attributes == null || scorePPCalculator == null) return 0d;

        var result = await scorePPCalculator.CalculateAsync(score, starDiff.Value.Attributes, cancellationToken).ConfigureAwait(false);
        return result.Total;
    }

    private void onPlayerScreen(Screens.Play.Player player)
    {
        this.playerScreen = player;
        Hub.GetDataRoot().MenuValues.OsuState = OsuStates.PLAYING;

        Logging.Log("PLAYER!");

        player.DimmableStoryboard?.Add(healthProcessorAccessor = new HealthProcessorAccessor());
    }

    private void updateLeaderboard(IList<ScoreInfo> scoreInfos)
    {
        var list = new List<LeaderboardPlayer>();

        int index = 0;
        LeaderboardPlayer? ourPlayer = null;

        foreach (var scoreInfo in scoreInfos)
        {
            var lbp = new LeaderboardPlayer
            {
                Name = scoreInfo.RealmUser.Username,
                Score = (int)scoreInfo.TotalScore,
                Combo = scoreInfo.Combo,
                MaxCombo = scoreInfo.MaxCombo,
                Mods = "NM",
                Hit300 = scoreInfo.GetResultsPerfect(),
                Hit100 = scoreInfo.GetResultsGreat(),
                Hit50 = scoreInfo.Statistics.GetValueOrDefault(HitResult.Meh, 0),
                HitMiss = scoreInfo.Statistics.GetValueOrDefault(HitResult.Miss, 0),
                Team = 0,
                Position = index
            };

            index++;

            if (lbp.Name == playerName)
                ourPlayer = lbp;
            else
                list.Add(lbp);
        }

        var dataRoot = Hub.GetDataRoot();
        dataRoot.GameplayValues.Leaderboard.Slots = list.ToArray();
        dataRoot.GameplayValues.Leaderboard.OurPlayer = ourPlayer;
    }

    private partial class HealthProcessorAccessor : CompositeDrawable
    {
        [Resolved]
        private HealthProcessor healthProcessor { get; set; } = null!;

        public HealthProcessor HealthProcessor => healthProcessor;
    }

    private partial class CounterContainer : CompositeDrawable
    {
        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
            => dependencyContainer = new DependencyContainer(base.CreateChildDependencies(parent));

        private DependencyContainer dependencyContainer = null!;
        private readonly GameplayState gameplayState;
        private readonly ScoreProcessor scoreProcessor;

        public CounterContainer(GameplayState gameplayState, ScoreProcessor scoreProcessor)
        {
            this.gameplayState = gameplayState;
            this.scoreProcessor = scoreProcessor;
        }

        private readonly PerformancePointsCounter counter = new PerformancePointsCounter
        {
            AlwaysPresent = true
        };

        [BackgroundDependencyLoader]
        private void load(OsuGame game)
        {
            dependencyContainer.CacheAs(typeof(GameplayState), gameplayState);
            dependencyContainer.CacheAs(typeof(ScoreProcessor), scoreProcessor);

            this.AlwaysPresent = true;
            this.Alpha = 0;

            this.AddInternal(counter);
        }

        public Bindable<int> Current => counter.Current;
    }
}
