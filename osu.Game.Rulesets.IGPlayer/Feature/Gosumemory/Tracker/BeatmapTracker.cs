using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Database;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Tracker;

public partial class BeatmapTracker : AbstractTracker
{
    public BeatmapTracker(TrackerHub hub)
        : base(hub)
    {
    }

    private readonly Bindable<WorkingBeatmap> beatmap = new Bindable<WorkingBeatmap>();
    private readonly IBindable<IReadOnlyList<Mod>> mods = new Bindable<IReadOnlyList<Mod>>();

    [Resolved]
    private IBindable<IReadOnlyList<Mod>> globalMods { get; set; } = null!;

    [Resolved]
    private BeatmapDifficultyCache beatmapDifficultyCache { get; set; } = null!;

    private GosuRealmDirectAccessor? directAccessor;

    [BackgroundDependencyLoader]
    private void load(Bindable<WorkingBeatmap> globalBeatmap)
    {
        this.beatmap.BindTo(globalBeatmap);
        this.mods.BindTo(globalMods);

        directAccessor = new GosuRealmDirectAccessor(realmAccess);
        AddInternal(directAccessor);
    }

    private string staticRoot()
    {
        return storage.GetFullPath("static", true);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        this.beatmap.BindValueChanged(e =>
        {
            this.onBeatmapChanged(e.NewValue);
        }, true);

        this.mods.BindValueChanged(e =>
        {
            this.onModsChanged(e.NewValue);
        }, true);
    }

    private void onModsChanged(IReadOnlyList<Mod> mods)
    {
        var currentWorking = beatmap.Value;
        var difficulty = new BeatmapDifficulty(currentWorking.BeatmapInfo.Difficulty);
        double timeRate = 1;

        foreach (var mod in mods)
        {
            switch (mod)
            {
                case IApplicableToDifficulty applicableToDifficulty:
                    applicableToDifficulty.ApplyToDifficulty(difficulty);
                    break;

                case IApplicableToRate applicableToRate:
                    timeRate = applicableToRate.ApplyToRate(0, timeRate);
                    break;
            }
        }

        Ruleset ruleset = currentWorking.BeatmapInfo.Ruleset.CreateInstance();

        var adjusted = ruleset.GetRateAdjustedDisplayDifficulty(difficulty, timeRate);

        var dataRoot = Hub.GetDataRoot();

        dataRoot.MenuValues.GosuBeatmapInfo.Stats.AR = adjusted.ApproachRate;
        dataRoot.MenuValues.GosuBeatmapInfo.Stats.CS = adjusted.CircleSize;
        dataRoot.MenuValues.GosuBeatmapInfo.Stats.HP = adjusted.DrainRate;
        dataRoot.MenuValues.GosuBeatmapInfo.Stats.OD = adjusted.OverallDifficulty;
    }

    private IBindable<StarDifficulty?>? starDifficulty;

    private CancellationTokenSource cancellationTokenSource;

    private void onBeatmapChanged(WorkingBeatmap newBeatmap)
    {
        Hub.GetDataRoot().UpdateBeatmap(newBeatmap);

        Logger.Log($"~BACKGROUND IS {newBeatmap.Metadata.BackgroundFile}");
        updateFileSupporters(newBeatmap.BeatmapSetInfo, newBeatmap);

        this.onModsChanged(this.mods.Value);

        // Beatmap star difficulty
        cancellationTokenSource?.Cancel();
        cancellationTokenSource = new CancellationTokenSource();

        this.starDifficulty?.UnbindAll();
        this.starDifficulty = null;
        this.starDifficulty = beatmapDifficultyCache.GetBindableDifficulty(newBeatmap.BeatmapInfo, cancellationTokenSource.Token);
        this.starDifficulty.BindValueChanged(e =>
        {
            double newVal = e.NewValue?.Stars ?? -1d;
            Hub.GetDataRoot().MenuValues.GosuBeatmapInfo.Stats.SR = (float)newVal;
        }, true);
    }

    [Resolved]
    private RealmAccess realmAccess { get; set; } = null!;

    [Resolved]
    private Storage storage { get; set; } = null!;

    // From LegacyExporter in OsuGame
    private void updateFileSupporters(BeatmapSetInfo setInfo, WorkingBeatmap beatmap)
    {
        if (directAccessor == null)
            return;

        string root = staticRoot();

        if (directAccessor == null) return;

        string? final = directAccessor.ExportFileSingle(setInfo, beatmap.Metadata.BackgroundFile, $"{root}/cover_{beatmap.GetHashCode()}");

        if (final == null) return;

        Logger.Log("~~~PUSH TO GOSU!");
        var dataRoot = Hub.GetDataRoot();

        string boardcast = final.Replace(root, "").Replace("/", "");
        Logger.Log("~~~BOARDCAST IS " + boardcast);
        dataRoot.MenuValues.GosuBeatmapInfo.Path.BackgroundPath = boardcast;
        dataRoot.MenuValues.GosuBeatmapInfo.Path.BgPath = boardcast;

        try
        {
            var server = Hub.GetWsLoader()?.Server;
            server?.AddStaticContent(staticRoot(), "/Songs");
        }
        catch (Exception e)
        {
            Logging.LogError(e, "Unable to add cache");
        }
    }
}
