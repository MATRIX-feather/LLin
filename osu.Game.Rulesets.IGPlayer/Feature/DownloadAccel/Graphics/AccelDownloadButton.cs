using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osu.Game.Graphics.Containers;
using osu.Game.Online;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Overlays.BeatmapSet.Buttons;
using osu.Game.Rulesets.IGPlayer.Helper.Injectors;
using osuTK.Graphics;

namespace osu.Game.Rulesets.IGPlayer.DownloadAccel.Graphics;

public partial class AccelDownloadButton : HeaderDownloadButton
{
    public AccelDownloadButton(APIBeatmapSet beatmapSet, bool noVideo = false)
        : base(beatmapSet, noVideo)
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        this.beatmapSet = beatmapSet;
        this.noVideo = noVideo;
    }

    private readonly APIBeatmapSet beatmapSet;
    private readonly bool noVideo;

    [BackgroundDependencyLoader]
    private void load()
    {
        try
        {
            var thisAsHeaderButton = (this as HeaderDownloadButton);
            var baseButton = (HeaderButton) thisAsHeaderButton.FindInstance(typeof(HeaderButton));
            var downloadTracker = (BeatmapDownloadTracker)thisAsHeaderButton.FindInstance(typeof(BeatmapDownloadTracker));
            var shakeContainer = (ShakeContainer)thisAsHeaderButton.FindInstance(typeof(ShakeContainer));
            var beatmaps = PreviewTrackInjector.AccelBeatmapModelDownloader;

            baseButton.BackgroundColour = Color4.Teal;

            baseButton.Action = () =>
            {
                try
                {
                    if (downloadTracker.State.Value != DownloadState.NotDownloaded)
                        shakeContainer.Shake();
                    else
                        beatmaps.Download(this.beatmapSet, this.noVideo);
                }
                catch (Exception e)
                {
                    Logging.LogError(e, "无法启动加速下载");
                }
            };

            this.tracker = new AccelBeatmapDownloadTracker(this.beatmapSet);
            tracker.State.BindValueChanged(v =>
            {
                ((Bindable<DownloadState>)downloadTracker.State).Value = v.NewValue;
            }, true);

            shakeContainer.Add(new AccelDownloadProgressBar(this.beatmapSet)
            {
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft
            });
            AddInternal(this.tracker);
        }
        catch (Exception e)
        {
            Logging.LogError(e, "无法设置加速下载动作，将使用原版下载器...");
        }
    }

    private DownloadTracker<IBeatmapSetInfo> tracker;
}
