using System;
using System.Linq;
using System.Threading;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Database;
using osu.Game.Online.API;
using osu.Game.Online.Leaderboards;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Scoring;
using osu.Game.Screens.Play.Leaderboards;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Replay;

public partial class BackgroundReplayPlugin : LLinPlugin
{
    public BackgroundReplayPlugin(LLinPluginProvider provider)
        : base(provider)
    {
        Flags.Add(PluginFlags.CanDisable);
        Depth = float.MaxValue;
    }

    public override ContentLayer Target => ContentLayer.Foreground;

    private ScoreManager scoreManager = null!;

    [Resolved]
    private RealmAccess realmAccess { get; set; } = null!;

    [Resolved]
    private RulesetStore rulesetStore { get; set; } = null!;

    [Resolved]
    private BeatmapManager beatmapManager { get; set; } = null!;

    [Resolved]
    private Storage storage { get; set; } = null!;

    [Resolved]
    private IAPIProvider api { get; set; } = null!;

    private LeaderboardManager leaderboardManager;

    private Drawable? displayingReplay;
    private Container? contentContainer;

    private Score? viewingScore;

    public Score? ViewingScore
    {
        get => viewingScore;
        set
        {
            viewingScore = value;
            loadReplay(value);
        }
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        this.leaderboardManager = new LeaderboardManager();
        Add(leaderboardManager);

        this.scoreManager = new ScoreManager(rulesetStore, () => beatmapManager, storage, realmAccess, api);
        RelativeSizeAxes = Axes.Both;
    }

    protected override void LoadComplete()
    {
        leaderboardManager.Scores.BindValueChanged(onScoresRefreshed);
        LLin!.OnBeatmapChanged(this.onBeatmapChanged, this, true);

        base.LoadComplete();
    }

    private void onBeatmapChanged(WorkingBeatmap beatmap)
    {
        removeCurrentReplayFromStage();

        var cr = new LeaderboardCriteria(beatmap.BeatmapInfo,
            beatmap.BeatmapInfo.Ruleset,
            BeatmapLeaderboardScope.Local,
            null);

        leaderboardManager.FetchWithCriteria(cr);
    }

    private void onScoresRefreshed(ValueChangedEvent<LeaderboardScores?> e)
    {
        var newValue = e.NewValue;
        if (newValue == null) return;

        ViewingScore = pickScore(newValue);
    }

    /// <summary>
    /// Pick a score from the given <see cref="LeaderboardScores"/>
    /// </summary>
    /// <returns>A <see cref="Score"/>, null if none found.</returns>
    private Score? pickScore(LeaderboardScores scores)
    {
        var currentBeatmap = LLin!.Beatmap.Value;

        Score? targetScore = scores.AllScores.Select(scoreInfo => scoreManager.GetScore(scoreInfo))
                                   .FirstOrDefault();

        if (targetScore == null) return null;

        // Prevent selecting scores that does not match the current beatmap
        if (!targetScore.ScoreInfo.BeatmapInfo?.Equals(currentBeatmap?.BeatmapInfo ?? null) ?? false)
            return null;

        return targetScore;
    }

    /// <summary>
    /// Remove the current playing replay from the stage.
    /// </summary>
    private void removeCurrentReplayFromStage()
    {
        var currentReplay = this.displayingReplay;

        if (currentReplay == null)
            return;

        currentReplay.Hide();
        currentReplay.Expire();

        displayingReplay = null;
    }

    public override bool Enable()
    {
        contentContainer?.FadeIn(300);
        loadReplay(ViewingScore);

        return base.Enable();
    }

    public override bool Disable()
    {
        contentContainer?.FadeOut(300);

        return base.Disable();
    }

    private CancellationTokenSource? replayLoadingCancellationTokenSource;

    /// <summary>
    ///
    /// </summary>
    /// <param name="viewingScore">The score to view, leave null for autoplay for the current beatmap</param>
    private void loadReplay(Score? viewingScore)
    {
        removeCurrentReplayFromStage();

        replayLoadingCancellationTokenSource?.Cancel();
        replayLoadingCancellationTokenSource = new CancellationTokenSource();

        var currentBeatmap = LLin!.Beatmap.Value;
        viewingScore ??= tryAutoMod(currentBeatmap);

        if (viewingScore == null) return;

        LoadComponentAsync(new ReplayContainer(viewingScore, LLin!)
        {
            RelativeSizeAxes = Axes.Both
        }, loaded =>
        {
            if (contentContainer == null) return;

            this.displayingReplay = loaded;
            contentContainer.Add(loaded);
            loaded.Show();
        }, replayLoadingCancellationTokenSource.Token);
    }

    private Score? tryAutoMod(WorkingBeatmap workingBeatmap)
    {
        var ruleset = workingBeatmap.BeatmapInfo.Ruleset.CreateInstance();
        var autoPlay = ruleset.GetAutoplayMod();

        var playableBeatmap = workingBeatmap.GetPlayableBeatmap(ruleset.RulesetInfo);

        // Using `autoPlay?.CreateScoreFromReplayData(workingBeatmap.Beatmap, []);` would produce a replay with invalid BeatmapInfo, why?
        var replay = autoPlay?.CreateReplayData(playableBeatmap, []);
        if (replay == null) return null;

        return new Score
        {
            Replay = replay.Replay,
            ScoreInfo = new ScoreInfo(playableBeatmap.BeatmapInfo, ruleset.RulesetInfo)
            {
                Date = DateTimeOffset.Now
            }
        };
    }

    protected override Drawable CreateContent()
    {
        return new Container
        {
            RelativeSizeAxes = Axes.Both,
            Name = "Replay player container"
        };
    }

    protected override bool OnContentLoaded(Drawable content)
    {
        this.contentContainer = content as Container;

        return true;
    }

    protected override bool PostInit()
    {
        return true;
    }
}
