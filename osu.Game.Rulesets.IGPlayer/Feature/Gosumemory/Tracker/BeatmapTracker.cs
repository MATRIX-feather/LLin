using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Database;
using osu.Game.Rulesets.IGPlayer.Helper.Configuration;
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

    private readonly Bindable<int> maxCacheSize = new Bindable<int>(300);

    [BackgroundDependencyLoader]
    private void load(Bindable<WorkingBeatmap> globalBeatmap, MConfigManager config)
    {
        this.beatmap.BindTo(globalBeatmap);
        this.mods.BindTo(globalMods);

        directAccessor = new GosuRealmDirectAccessor(realmAccess);
        AddInternal(directAccessor);

        config.BindWith(MSetting.GosuMaximumCacheSize, maxCacheSize);

        //host.Exited += () => clearCache(staticRoot());
    }

    private string staticRoot()
    {
        string? path = storage.GetFullPath("gosu_caches", true);

        try
        {
            if (!Path.Exists(path))
                Directory.CreateDirectory(path);
        }
        catch (Exception e)
        {
            Logging.LogError(e, "Unable to create statics directory");
        }

        return path;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        this.beatmap.BindValueChanged(e =>
        {
            this.onBeatmapChanged(e.NewValue);
            ((Bindable<IReadOnlyList<Mod>>)mods).TriggerChange();
        }, true);

        this.mods.BindValueChanged(e =>
        {
            this.onModsChanged(e.NewValue);
        }, true);
    }

    [Resolved]
    private Bindable<RulesetInfo> globalRuleset { get; set; } = null!;

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

        Ruleset ruleset = currentWorking.BeatmapInfo.Ruleset.Available
            ? currentWorking.BeatmapInfo.Ruleset.CreateInstance()
            : globalRuleset.Value.CreateInstance();

        BeatmapDifficulty adjusted = difficulty;

        try
        {
            adjusted = ruleset.GetRateAdjustedDisplayDifficulty(difficulty, timeRate);
        }
        catch (Exception)
        {
            Logging.Log("Can't get adjusted difficulty, using original one...");
        }

        var dataRoot = Hub.GetDataRoot();

        dataRoot.MenuValues.GosuBeatmapInfo.Stats.BPM.Max = (int)Math.Round(beatmap.Value.Beatmap.ControlPointInfo.BPMMaximum * timeRate);
        dataRoot.MenuValues.GosuBeatmapInfo.Stats.BPM.Min = (int)Math.Round(beatmap.Value.Beatmap.ControlPointInfo.BPMMinimum * timeRate);

        dataRoot.MenuValues.GosuBeatmapInfo.Stats.AR = adjusted.ApproachRate;
        dataRoot.MenuValues.GosuBeatmapInfo.Stats.CS = adjusted.CircleSize;
        dataRoot.MenuValues.GosuBeatmapInfo.Stats.HP = adjusted.DrainRate;
        dataRoot.MenuValues.GosuBeatmapInfo.Stats.OD = adjusted.OverallDifficulty;
    }

    private IBindable<StarDifficulty?>? starDifficulty;

    private CancellationTokenSource cancellationTokenSource;

    private void onBeatmapChanged(WorkingBeatmap newBeatmap)
    {
        Hub.GetDataRoot().UpdateMetadata(newBeatmap);

        //Logging.Log($"~BACKGROUND IS {newBeatmap.Metadata.BackgroundFile}");
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
            double newVal = e.NewValue?.Stars ?? 0d;

            var dataRoot = Hub.GetDataRoot();
            dataRoot.MenuValues.GosuBeatmapInfo.Stats.SR = (float)newVal;
            dataRoot.MenuValues.GosuBeatmapInfo.Stats.MaxCombo = e.NewValue?.MaxCombo ?? 0;
        }, true);
    }

    [Resolved]
    private RealmAccess realmAccess { get; set; } = null!;

    [Resolved]
    private Storage storage { get; set; } = null!;

    private CancellationTokenSource fileExportCancellationTokenSource;

    private void updateFileSupporters(BeatmapSetInfo setInfo, WorkingBeatmap beatmap)
    {
        if (directAccessor == null)
            return;

        string root = staticRoot();

        // Cancel previous update process
        fileExportCancellationTokenSource?.Cancel();
        fileExportCancellationTokenSource = new CancellationTokenSource();

        Task.Run(async () =>
        {
            await Task.Run(() => ensureCacheNotTooLarge(root)).ConfigureAwait(false);

            string prefix = beatmap.BeatmapInfo.OnlineID != -1
                ? $"{beatmap.BeatmapInfo.OnlineID}"
                : $"L_{beatmap.BeatmapInfo.MD5Hash[..(Math.Min(8, beatmap.BeatmapInfo.MD5Hash.Length))]}";

            // Background
            string backgroundExt = "";
            string[] rawNameSplit = beatmap.Metadata.BackgroundFile?.Split('.') ?? new string[]{};
            backgroundExt = rawNameSplit.Length >= 2 ? rawNameSplit[^1] : "";

            string backgroundDesti = "_default.png";
            if (beatmap.Metadata.BackgroundFile != null) //Yes, this can be null
                backgroundDesti = $"{root}/{prefix}_{genGuidFrom(beatmap.Metadata.BackgroundFile)}.{backgroundExt}";

            string? backgroundFinal = await directAccessor.ExportSingleTask(
                setInfo,
                beatmap.Metadata.BackgroundFile ?? "",
                backgroundDesti).ConfigureAwait(false);

            // .osu File
            string osuFileDesti = "_default.osz";
            if (beatmap.BeatmapInfo.File?.Filename != null)
                osuFileDesti = $"{root}/{prefix}_{genGuidFrom(beatmap.BeatmapInfo.File.Filename)}.osu";

            string? osuFileFinal = await directAccessor.ExportSingleTask(
                setInfo,
                beatmap.BeatmapInfo.File?.Filename ?? "",
                osuFileDesti).ConfigureAwait(false);

            // Audio file
            string audioFileDesti = "_default.mp3";

            if (beatmap.Metadata?.AudioFile != null)
            {
                string[] audioNameSpilt = beatmap.Metadata.AudioFile.Split(".");
                string audioExtName = audioNameSpilt.Length >= 2 ? audioNameSpilt[^1] : "audio";

                audioFileDesti = $"{root}/{prefix}_{genGuidFrom(beatmap.Metadata.AudioFile)}.{audioExtName}";
            }

            string? audioFinal = await directAccessor.ExportSingleTask(
                setInfo,
                beatmap.BeatmapInfo.BeatmapSet?.Metadata.AudioFile ?? "",
                audioFileDesti).ConfigureAwait(false);

            // Await for statics refresh
            await Task.Run(updateStatics).ConfigureAwait(false);

            // Update!
            this.Schedule(() =>
            {
                //Logging.Log("~~~PUSH TO GOSU!");
                var dataRoot = Hub.GetDataRoot();

                if (backgroundFinal != null)
                {
                    string boardcast = backgroundFinal.Replace(root, "").Replace("/", "");
                    //Logging.Log("~~~BOARDCAST IS " + boardcast);
                    dataRoot.MenuValues.GosuBeatmapInfo.Path.BackgroundPath = boardcast;
                    dataRoot.MenuValues.GosuBeatmapInfo.Path.BgPath = boardcast;
                }

                if (osuFileFinal != null)
                    dataRoot.MenuValues.GosuBeatmapInfo.Path.BeatmapFile = osuFileFinal.Replace(root, "").Replace("/", "");

                if (audioFinal != null)
                    dataRoot.MenuValues.GosuBeatmapInfo.Path.AudioPath = audioFinal.Replace(root, "").Replace("/", "");
            });
        }, fileExportCancellationTokenSource.Token);
    }

    private void ensureCacheNotTooLarge(string cachePath)
    {
        if (!Path.Exists(cachePath)) return;

        var files = new DirectoryInfo(cachePath).GetFiles();
        long totalMegabytes = files.Sum(fileInfo => fileInfo.Length) / (1024 * 1024); // (1(byte) * 1024(KiB) * 1024(MiB))

        //Logging.Log("Cache size is " + totalMegabytes + "MiB");

        if (totalMegabytes <= maxCacheSize.Value) return;

        clearCache(cachePath);
    }

    private void clearCache(string cacheRoot)
    {
        try
        {
            var dirInfo = new DirectoryInfo(cacheRoot);
            if (!dirInfo.Exists) return;

            foreach (var fileInfo in dirInfo.GetFiles())
                fileInfo.Delete();
        }
        catch (Exception e)
        {
            Logging.Log("Error occurred while clearing gosu cache... Not a big deal, maybe?");
            Logging.Log(e.Message);
            Logging.Log(e.StackTrace ?? "<No stacktrace>");
        }
    }

    private Guid genGuidFrom(string str)
    {
        byte[] hash = MD5.HashData(Encoding.Unicode.GetBytes(str));
        return new Guid(hash);
    }

    private void updateStatics()
    {
        try
        {
            var server = Hub.GetWsLoader()?.Server;
            server?.RemoveStaticContent(staticRoot());
            server?.AddStaticContent(staticRoot(), "/Songs");
        }
        catch (Exception e)
        {
            Logging.LogError(e, "Unable to add cache");
        }
    }
}
