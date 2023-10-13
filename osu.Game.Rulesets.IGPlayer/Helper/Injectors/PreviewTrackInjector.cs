using System;
using System.Reflection;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Bindables;
using osu.Framework.Development;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osu.Framework.Logging;
using osu.Framework.Utils;
using osu.Game.Audio;
using osu.Game.Beatmaps;
using osu.Game.Database;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.API;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Overlays;
using osu.Game.Rulesets.IGPlayer.Feature.DownloadAccel;
using osu.Game.Rulesets.IGPlayer.Feature.DownloadAccel.Graphics;
using osu.Game.Tests.Visual;
using osuTK;
using Realms;

namespace osu.Game.Rulesets.IGPlayer.Helper.Injectors;

public partial class PreviewTrackInjector : AbstractInjector
{
    private readonly object injectLock = new();

    private ThreadSafeReference.List<OsuFocusedOverlayContainer> focusedContainers;

    [Resolved]
    private PreviewTrackManager previewTrackManager { get; set; } = null!;

    [Resolved]
    private OsuGame game { get; set; } = null!;

    [Resolved(canBeNull: true)]
    private BeatmapManager? beatmapManager { get; set; }

    [Resolved(canBeNull: true)]
    private IAPIProvider apiProvider { get; set; } = null!;

    private APIBeatmapSet? currentApiBeatmapSet;

    [BackgroundDependencyLoader]
    private void load(AudioManager audio, TextureStore textures, INotificationOverlay notificationOverlay)
    {
        previewTrack.BindValueChanged(this.onPreviewTrackChanged);

        try
        {
            if (!locateOverlays())
                Logger.Log("无法定位到PreviewTrackManager", level: LogLevel.Important);

            if (AccelBeatmapModelDownloader == null)
            {
                SetupAccelDownloader(beatmapManager, apiProvider);
                AccelBeatmapModelDownloader.attachOsuGame(notificationOverlay);
            }

#if DEBUG
            game.Add(new OsuAnimatedButton
            {
                Size = new Vector2(120),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    new OsuSpriteText()
                    {
                        Text = "New track",
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Font = OsuFont.GetFont(size: 24)
                    }
                },
                Action = () =>
                {
                    var apiSet = new APIBeatmapSet
                    {
                        HasVideo = RNG.Next(0, 1000) < 500,
                        OnlineID = 1247651 + RNG.Next(0, 100),
                        Title = "Anemone",
                        TitleUnicode = "Anomone (Unicode)",
                        Artist = "DUSTCELL",
                        Author = new APIUser
                        {
                            Username = "Sparhtend"
                        },
                        Covers = new BeatmapSetOnlineCovers
                        {
                            Card = "https://a.sayobot.cn/beatmaps/1247651/covers/cover.jpg",
                            CardLowRes = "https://a.sayobot.cn/beatmaps/1247651/covers/cover.jpg"
                        }
                    };

                    previewTrack.Value = new PreviewTrackManager.TrackManagerPreviewTrack(apiSet, new OsuTestScene.ClockBackedTestWorkingBeatmap.TrackVirtualStore(this.Clock));
                }
            });
#endif
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
            // 不在Debug模式下自动更新previewTrack
            if (previewTrackFieldInfo != null && !DebugUtils.IsDebugBuild)
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
        prevContainer?.Hide();
        prevContainer?.Expire();

        if (track == null)
        {
            currentApiBeatmapSet = null;
            return;
        }

        var apiSet = getAPISet(track);
        if (apiSet.Equals(currentApiBeatmapSet)) return;
        currentApiBeatmapSet = apiSet;

        //apiSet.TitleUnicode = "标题Unicodeeeeeeeeeeeeeeeeeeeeeeeeeeeeee测试";
        //apiSet.Title = "Title Romanist";

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
