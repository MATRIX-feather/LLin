using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osu.Framework.Timing;
using osu.Game.Beatmaps;
using osu.Game.Online.API;
using osu.Game.Overlays;
using osu.Game.Overlays.SkinEditor;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Data.Consts;
using osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Data.Gameplay;
using osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Extensions;
using osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.PP;
using osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Web;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;
using osu.Game.Screens;
using osu.Game.Screens.Play;
using osu.Game.Screens.Play.HUD;
using osu.Game.Screens.Ranking;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory
{
    public partial class Updater : CompositeDrawable
    {
        private readonly WsLoader wsLoader;

        public Updater(WsLoader game)
        {
            this.wsLoader = game;
        }

        [Resolved]
        private OsuGame game { get; set; } = null!;

        public void LocateScreenStack()
        {
            if (screenStack != null) return;

            const BindingFlags flag = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic;
            var screenStackField = game.GetType().GetFields(flag).FirstOrDefault(f => f.FieldType == typeof(OsuScreenStack));

            if (screenStackField == null) return;

            object? val = screenStackField.GetValue(game);

            if (val is not OsuScreenStack osuScreenStack) return;

            this.screenStack = osuScreenStack;

            screenStack.ScreenExited += onScreenSwitch;
            screenStack.ScreenPushed += onScreenSwitch;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            if (screenStack == null)
                LocateScreenStack();

            if (this.screenStack == null)
                Logger.Log("无法定位到OsuScreenStack, 一些功能可能不会正常运作", level: LogLevel.Important);

            Logger.Log("Gosumemoty Compat!");

            this.Clock = new FramedClock(null, false);

            var obj = wsLoader.DataRoot;

            workingBeatmap.BindValueChanged(v =>
            {
                beatmapChangedTime = Clock.CurrentTime;

                if (!isInGame()) updateOverallPerformancePoints(workingBeatmap.Value, v.OldValue != v.NewValue);
                obj.UpdateBeatmap(v.NewValue);
            });

            ruleset.BindValueChanged(v =>
            {
                this.rsInstance = v.NewValue.CreateInstance();
                this.performanceCalculator = rsInstance.CreatePerformanceCalculator();
                wsLoader.DataRoot.GameplayValues.Gamemode = LegacyGamemodes.FromRulesetInfo(v.NewValue);

                workingBeatmap.TriggerChange();
            }, true);

            mods.BindValueChanged(v =>
            {
                obj.MenuValues.Mods.UpdateFrom(v.NewValue);
                workingBeatmap.TriggerChange();
            });

            updateOverallPerformancePoints(workingBeatmap.Value);
        }

        private OsuScreenStack? screenStack;

        [Resolved]
        private IBindable<IReadOnlyList<Mod>> mods { get; set; } = null!;

        [Resolved]
        private MusicController musicController { get; set; } = null!;

        [Resolved]
        private Bindable<WorkingBeatmap> workingBeatmap { get; set; } = null!;

        [Resolved]
        private OsuGame osuGame { get; set; } = null!;

        [Resolved]
        private Bindable<RulesetInfo> ruleset { get; set; } = null!;

        private double lastUpdate;

        protected override void Update()
        {
            base.Update();

            //更新太快容易卡住网页
            if (Clock.CurrentTime - lastUpdate < 200) return;

            lastUpdate = Clock.CurrentTime;
            UpdateValues();
            if (scheduleStrainComputes) updatePPStrains(workingBeatmap.Value);
        }

        //region InGame and PP

        private CounterContainer? inGamePPCounter;

        private int maxPP = 0;
        private int performanceThisRun = 0;

        [Resolved]
        private ScoreManager scoreManager { get; set; } = null!;

        [Resolved]
        private ScorePerformanceCache scorePerformanceCache { get; set; } = null!;

        private CancellationTokenSource? overallPPCancellationTokenSource;

        private void updateOverallPerformancePoints(WorkingBeatmap workingBeatmap, bool recalculateStrains = true)
        {
            // 排除Classic模组（会导致最大pp不准确）
            var modsCopy = mods.Value.Where(m => m.Acronym != "CL")
                               .Select(m => m.DeepClone()).ToArray();

            overallPPCancellationTokenSource?.Cancel();
            overallPPCancellationTokenSource = new CancellationTokenSource();

            scheduleStrainComputes = true;

            Task.Run(async () => await updateOverallPPTask(workingBeatmap, modsCopy), overallPPCancellationTokenSource.Token);
        }

        private Task updateOverallPPTask(WorkingBeatmap workingBeatmap, Mod[] modsCopy)
        {
            int maxpp = 0;
            var obj = wsLoader.DataRoot;

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
            catch (Exception)
            {
            }

            this.maxPP = maxpp;
            obj.GameplayValues.pp.MaxThisPlay = obj.GameplayValues.pp.PPIfFc = maxpp;
            obj.MenuValues.pp.PPPerfect = maxpp;

            return Task.CompletedTask;
        }

        private Ruleset? rsInstance;

        private CancellationTokenSource? ppStrainCancellationTokenSource;

        private bool scheduleStrainComputes;

        private double beatmapChangedTime;

        private void updatePPStrains(WorkingBeatmap workingBeatmap)
        {
            ppStrainCancellationTokenSource?.Cancel();
            ppStrainCancellationTokenSource = new CancellationTokenSource();

            Task.Run(() =>
            {
                try
                {
                    double length = workingBeatmap.Track.Length;

                    //WorkingBeatmap.TrackLoaded: true + WorkingBeatmap.Track.IsLoaded: false -> Track Length: 0
                    if (length <= 0)
                    {
                        //持续5秒都没有音频，可能已经损坏，清空分布
                        //todo: 没有音频的时候使用谱面长度来计算并更新分布和进度
                        if (Clock.CurrentTime - beatmapChangedTime >= 5 * 1000)
                        {
                            wsLoader.DataRoot.MenuValues.pp.Strains = new[] { 0f };
                            return;
                        }

                        scheduleStrainComputes = true;
                        return;
                    }

                    scheduleStrainComputes = false;

                    // 最大分段数和密度缩放
                    int maximumSegments = 512;
                    double segmentScale = 1;

                    // 根据歌曲长度分段，每 (1 * segScale) 秒分一段
                    int targetSegments = (int)(TimeSpan.FromMilliseconds(length).TotalSeconds * segmentScale);
                    targetSegments = Math.Min(maximumSegments, targetSegments);
                    if (targetSegments <= 0) targetSegments = 1;

                    // 尝试自动转谱
                    var converter = workingBeatmap.BeatmapInfo.Ruleset.CreateInstance().CreateBeatmapConverter(workingBeatmap.Beatmap);
                    IBeatmap? beatmap = null;

                    //Logger.Log($"Track length: {length} ~ Segments {targetSegments} ~ Conv? {converter.CanConvert()} ~ Loaded? {workingBeatmap.Track.IsLoaded} ~ Track? {workingBeatmap.Track}");
                    if (converter.CanConvert()) beatmap = converter.Convert();
                    var hitObjects = beatmap?.HitObjects ?? new HitObject[] { };

                    //获取每段的音频跨度
                    double audioStep = length / targetSegments;

                    //Segment -> Count
                    var segments = new Dictionary<int, float>();

                    for (int i = 0; i < targetSegments; i++)
                    {
                        //此段的音频跨度
                        double startTime = i * audioStep;
                        double endTime = (1 + i) * audioStep;

                        //将跨度内的所有物件添加进来
                        //o -> [startTime, endTime)
                        int count = hitObjects.Count(o => o.StartTime < endTime && o.StartTime >= startTime);

                        segments.TryAdd(i, count);
                    }

                    //最后添加到DataRoot里
                    wsLoader.DataRoot.MenuValues.pp.Strains = segments.Values.ToArray();
                }
                catch (Exception e)
                {
                }
            }, ppStrainCancellationTokenSource.Token);
        }

        private PerformanceCalculator? performanceCalculator;

        //endregion

        private bool isInGame()
        {
            return playerScreen != null;
        }

        private Screens.Play.Player? playerScreen;

        private ResultsScreen? resultsScreen;

        private CancellationTokenSource? scorePPCalcTokenSource;

        private string playerName = "???";

        private void onScreenSwitch(IScreen prevScreen, IScreen nextScreen)
        {
            scorePPCalcTokenSource?.Cancel();

            this.playerScreen = null;
            this.resultsScreen = null;

            //updateCancellationTokenSource?.Cancel();

            var dataRoot = wsLoader.DataRoot;

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
                    var score = results.SelectedScore.Value;

                    if (score != null)
                    {
                        dataRoot.GameplayValues.FromScore(score);
                        dataRoot.GameplayValues.pp.Current = (int?)score.PP ?? 0;

                        if (score.PP.HasValue)
                        {
                            dataRoot.GameplayValues.pp.Current = (int)score.PP;
                        }
                        else
                        {
                            scorePPCalcTokenSource = new CancellationTokenSource();
                            scorePerformanceCache.CalculatePerformanceAsync(score, scorePPCalcTokenSource.Token)
                                                 .ContinueWith(t =>
                                                 {
                                                     if (screenStack?.CurrentScreen != results) return;

                                                     double? total = t.GetResultSafely<PerformanceAttributes?>()?.Total;
                                                     dataRoot.GameplayValues.pp.Current = (int?)total ?? 0;
                                                 });
                        }

                        dataRoot.MenuValues.Mods.UpdateFrom(score.Mods.Where(m => m.Acronym != "CL").ToArray());
                    }

                    dataRoot.MenuValues.OsuState = OsuStates.PLAYING;
                    this.resultsScreen = results;
                    break;

                case Screens.Play.Player player:
                    this.playerScreen = player;
                    dataRoot.MenuValues.OsuState = OsuStates.PLAYING;

                    player.DimmableStoryboard?.Add(healthProcessorAccessor = new HealthProcessorAccessor());
                    break;

                default:
                    dataRoot.MenuValues.OsuState = OsuStates.IDLE;
                    break;
            }

            dataRoot.GameplayValues.Name = playerName = (playerScreen != null)
                ? (playerScreen.Score?.ScoreInfo?.User.Username ?? "???")
                : (resultsScreen != null ? (resultsScreen.SelectedScore.Value?.User.Username ?? "???") : api.LocalUser.Value.Username);
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

            wsLoader.DataRoot.GameplayValues.Leaderboard.Slots = list.ToArray();
            wsLoader.DataRoot.GameplayValues.Leaderboard.OurPlayer = ourPlayer;
        }

        private HealthProcessorAccessor? healthProcessorAccessor;

        [Resolved]
        private IAPIProvider api { get; set; } = null!;

        //private CancellationTokenSource? updateCancellationTokenSource;

        public void UpdateValues()
        {
            this.AlwaysPresent = true;

            var obj = wsLoader.DataRoot;
            obj.UpdateTrack(musicController.CurrentTrack);

            if (isInGame())
            {
                var scoreInfo = playerScreen!.Score?.ScoreInfo.DeepClone();

                //scoreInfo in EndlessPlayer would be null for somehow
                if (scoreInfo == null)
                    return;

                playerScreen!.GameplayState.ScoreProcessor.PopulateScore(scoreInfo);

                obj.GameplayValues.FromScore(scoreInfo);

                if (inGamePPCounter == null)
                    AddInternal(inGamePPCounter = new CounterContainer(playerScreen.GameplayState, playerScreen.GameplayState.ScoreProcessor));

                double health = 200 * (healthProcessorAccessor?.HealthProcessor.Health.Value ?? 0d);
                obj.GameplayValues.Hp.Smooth = obj.GameplayValues.Hp.Normal;
                obj.GameplayValues.Hp.Normal = (float)health;

                performanceThisRun = inGamePPCounter.Current.Value;
                obj.GameplayValues.pp.Current = this.performanceThisRun;
            }

            if (inGamePPCounter != null && !isInGame())
            {
                inGamePPCounter.Expire();
                inGamePPCounter = null;
            }

            try
            {
                string str = JsonConvert.SerializeObject(obj, Formatting.None, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Include
                });

                this.wsLoader.Boardcast(str);
            }
            catch (Exception e)
            {
                Logger.Log($"Unable to update osu status to WebSocket: {e.Message}", level: LogLevel.Important);
                Logger.Log(e.ToString());
            }
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
}
