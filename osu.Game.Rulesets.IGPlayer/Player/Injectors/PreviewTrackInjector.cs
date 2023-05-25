using System;
using System.Reflection;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Bindables;
using osu.Framework.Development;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osu.Framework.Logging;
using osu.Game.Audio;
using osu.Game.Beatmaps;
using osu.Game.Database;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.API;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Overlays;
using osu.Game.Rulesets.IGPlayer.Player.DownloadAccel;
using osu.Game.Rulesets.IGPlayer.Player.Graphics;
using osu.Game.Tests.Visual;
using osuTK;
using Realms;

namespace osu.Game.Rulesets.IGPlayer.Player.Injectors;

public partial class PreviewTrackInjector : AbstractInjector
{
    private object injectLock = new();

    private ThreadSafeReference.List<OsuFocusedOverlayContainer> focusedContainers;

    [Resolved]
    private PreviewTrackManager previewTrackManager { get; set; }

    [Resolved]
    private OsuGame game { get; set; }

    [Resolved(canBeNull: true)]
    private BeatmapManager? beatmapManager { get; set; }

    [Resolved(canBeNull: true)]
    private IAPIProvider apiProvider { get; set; }

    [BackgroundDependencyLoader]
    private void load(AudioManager audio, TextureStore textures, INotificationOverlay notificationOverlay)
    {
        previewTrack.BindValueChanged(this.onPreviewTrackChanged);

        try
        {
            if(!locateOverlays())
                Logger.Log("无法定位到PreviewTrackManager", level: LogLevel.Important);

            if (AccelBeatmapModelDownloader == null)
            {
                SetupAccelDownloader(beatmapManager, apiProvider);
                AccelBeatmapModelDownloader.attachOsuGame(notificationOverlay);
            }

            if (DebugUtils.IsDebugBuild)
            game.Add(new OsuAnimatedButton
            {
                Size = new Vector2(120),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    new OsuSpriteText()
                    {
                        Text = "dddddddddddd"
                    }
                },
                Action = () =>
                {
                    var inf = new DummyWorkingBeatmap(audio, textures);
                    var apiSet = new APIBeatmapSet();

                    apiSet.HasVideo = true;
                    previewTrack.Value = new PreviewTrackManager.TrackManagerPreviewTrack(apiSet, new OsuTestScene.ClockBackedTestWorkingBeatmap.TrackVirtualStore(this.Clock));
                }
            });
        }
        catch (Exception e)
        {
            Logging.LogError(e, "无法定位到 PreviewTrackManager :(");
        }
    }

    private readonly Bindable<PreviewTrackManager.TrackManagerPreviewTrack> previewTrack = new();
    private FieldInfo? previewTrackFieldInfo;

    protected override void Update()
    {
        base.Update();

        try
        {
            if (previewTrackFieldInfo != null)
                previewTrack.Value = (PreviewTrackManager.TrackManagerPreviewTrack)previewTrackFieldInfo.GetValue(previewTrackManager);
        }
        catch (Exception e)
        {
            Logging.LogError(e, "未能获取当前预览歌曲");
            previewTrackFieldInfo = null;
        }
    }

    private bool locateOverlays()
    {
        lock (injectLock)
        {
            object? val;

            if (previewTrackFieldInfo == null)
            {
                //根据特征寻找字段
                var overlaysField = this.FindFieldInstance(previewTrackManager, typeof(PreviewTrackManager.TrackManagerPreviewTrack));

                if (overlaysField == null) return false;

                val = overlaysField.GetValue(previewTrackManager);
                //if (val is not PreviewTrackManager.TrackManagerPreviewTrack) throw new NullDependencyException("获取到的值不是PreviewTrack");

                this.previewTrackFieldInfo = overlaysField;
            }

            previewTrack.Value = (PreviewTrackManager.TrackManagerPreviewTrack)previewTrackFieldInfo.GetValue(previewTrackManager);
            return true;
        }
    }

    private APIBeatmapSet getAPISet(PreviewTrackManager.TrackManagerPreviewTrack previewTrack)
    {
        const BindingFlags flag = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic;
        APIBeatmapSet? val;

        var field = this.FindFieldInstance(previewTrack, typeof(IBeatmapSetInfo));
        if (field == null) return null;

        val = field.GetValue(previewTrack) as APIBeatmapSet;

        return val;
    }

    private void onPreviewTrackChanged(ValueChangedEvent<PreviewTrackManager.TrackManagerPreviewTrack> e)
    {
        Logger.Log($"🦢🦢 Preview track changed! {e.OldValue} -> {e.NewValue}");
        PreviewTrackManager.TrackManagerPreviewTrack? track = e.NewValue;
        if (track == null) return;

        var apiSet = getAPISet(track);

        prevContainer?.Expire();
        var container = new AccelOptionContainer(apiSet);
        prevContainer = container;
        game.Add(container);
    }

    private AccelOptionContainer? prevContainer;

    public static AccelBeatmapModelDownloader? AccelBeatmapModelDownloader;

    public static void SetupAccelDownloader(IModelImporter<BeatmapSetInfo> beatmapImporter, IAPIProvider api)
    {
        AccelBeatmapModelDownloader = new AccelBeatmapModelDownloader(beatmapImporter, api);
    }
}
