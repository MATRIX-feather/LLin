using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Database;
using osu.Game.Online.API;
using osu.Game.Online.Leaderboards;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SideBar.Settings.Items;
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

    private Drawable? displayingContent;
    private Score? viewingScore;

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
        expireCurrentReplay();

        var cr = new LeaderboardCriteria(beatmap.BeatmapInfo,
            beatmap.BeatmapInfo.Ruleset,
            BeatmapLeaderboardScope.Local,
            null);

        leaderboardManager.FetchWithCriteria(cr);
    }

    private void onScoresRefreshed(ValueChangedEvent<LeaderboardScores?> e)
    {
        Cancel();

        var newValue = e.NewValue;
        if (newValue == null) return;

        Score? targetScore = newValue.TopScores.Select(scoreInfo => scoreManager.GetScore(scoreInfo))
                                     .OfType<Score>()
                                     .FirstOrDefault();

        if (targetScore == null) return;

        this.viewingScore = targetScore;
        expireCurrentReplay();

        if (!Disabled.Value)
            cancelAndLoad();
    }

    private void expireCurrentReplay()
    {
        var currentReplay = this.displayingContent;

        if (currentReplay == null)
            return;

        currentReplay.Hide();
        currentReplay.Expire();

        this.Remove(currentReplay, true);

        displayingContent = null;
    }

    public override bool Disable()
    {
        expireCurrentReplay();
        return base.Disable();
    }

    public override bool Enable()
    {
        cancelAndLoad();

        return base.Enable();
    }

    private void cancelAndLoad()
    {
        Cancel();
        Load();
    }

    protected override Drawable CreateContent()
    {
        var score = viewingScore;
        if (score == null)
            return new PlaceHolder();

        return new ReplayContainer(score, LLin!)
        {
            RelativeSizeAxes = Axes.Both
        };
    }

    protected override bool OnContentLoaded(Drawable content)
    {
        this.displayingContent = content;
        content.FadeIn(300);

        return true;
    }

    protected override bool PostInit()
    {
        return true;
    }
}
