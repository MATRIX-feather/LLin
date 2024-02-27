using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Logging;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Tracker;

/// <summary>
/// 计算pp/密度表
/// </summary>
public partial class BeatmapStrainTracker : AbstractTracker
{
    public BeatmapStrainTracker(TrackerHub hub)
        : base(hub)
    {
    }

    private double invokeTime = 0d;
    private WorkingBeatmap? beatmapOnInvoke;

    private CancellationTokenSource? ppStrainCancellationTokenSource;

    private bool scheduleStrainComputes;

    private readonly Bindable<WorkingBeatmap> working = new Bindable<WorkingBeatmap>();

    [BackgroundDependencyLoader]
    private void load(Bindable<WorkingBeatmap> globalWorking)
    {
        this.working.BindTo(globalWorking);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        working.BindValueChanged(e =>
        {
            this.UpdateStrain(e.NewValue);
        });
    }

    protected override void Update()
    {
        if (scheduleStrainComputes && beatmapOnInvoke == working.Value)
            UpdateStrain(this.working.Value);
    }

    public void UpdateStrain(WorkingBeatmap workingBeatmap)
    {
        this.invokeTime = Clock.CurrentTime;
        beatmapOnInvoke = workingBeatmap;
        scheduleStrainComputes = false;

        ppStrainCancellationTokenSource?.Cancel();
        ppStrainCancellationTokenSource = new CancellationTokenSource();

        Task.Run(async () => await updateStrain(workingBeatmap, Hub.GetDataRoot().MenuValues.PP.Strains), ppStrainCancellationTokenSource.Token)
            .ContinueWith(task =>
            {
                if (!task.IsCompleted) return;

                if (task.Exception != null)
                {
                    Logging.LogError(task.Exception, "Error occurred while updating strain");
                    return;
                }

                this.Schedule(() =>
                {
                    float[] result = task.GetResultSafely();
                    Hub.GetDataRoot().MenuValues.PP.Strains = result;
                });
            });
    }

    [Resolved]
    private Bindable<RulesetInfo> globalRuleset { get; set; } = null!;

    private Task<float[]> updateStrain(WorkingBeatmap workingBeatmap, float[]? defaultVal = null)
    {
        defaultVal ??= new[] { 0f };

        try
        {
            double length = workingBeatmap.Track.Length;

            //WorkingBeatmap.TrackLoaded: true + WorkingBeatmap.Track.IsLoaded: false -> Track Length: 0
            if (length <= 0)
            {
                //持续5秒都没有音频，可能已经损坏，清空分布
                //todo: 没有音频的时候使用谱面长度来计算并更新分布和进度
                if (Clock.CurrentTime - invokeTime >= 10 * 1000)
                {
                    Logging.Log("谱面音频在10秒内都没有加载，将放弃计算物件分布...", level: LogLevel.Important);
                    return Task.FromResult(new[] { 0f });
                }

                scheduleStrainComputes = true;
                return Task.FromResult(defaultVal);
            }

            scheduleStrainComputes = false;

            // 最大分段数和密度缩放
            int maximumSegments = 512;
            double segmentScale = 1;

            // 根据歌曲长度分段，总共分为 (歌曲总时间(秒) * segScale) 段
            int targetSegments = (int)(TimeSpan.FromMilliseconds(length).TotalSeconds * segmentScale);

            // 限制最大分段数量
            targetSegments = Math.Min(maximumSegments, targetSegments);
            if (targetSegments <= 0) targetSegments = 1;

            var rulesetInstance = workingBeatmap.BeatmapInfo.Ruleset.Available
                ? workingBeatmap.BeatmapInfo.Ruleset.CreateInstance()
                : globalRuleset.Value.CreateInstance();

            // 尝试自动转谱
            var converter = rulesetInstance.CreateBeatmapConverter(workingBeatmap.Beatmap);
            IBeatmap? beatmap = null;

            //Logging.Log($"Track length: {length} ~ Segments {targetSegments} ~ Conv? {converter.CanConvert()} ~ Loaded? {workingBeatmap.Track.IsLoaded} ~ Track? {workingBeatmap.Track}");
            if (converter.CanConvert()) beatmap = converter.Convert();
            var hitObjects = beatmap?.HitObjects ?? Array.Empty<HitObject>();

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

            //最后将其返回
            return Task.FromResult(segments.Values.ToArray());
        }
        catch (Exception e)
        {
            return Task.FromException<float[]>(e);
        }
    }
}
