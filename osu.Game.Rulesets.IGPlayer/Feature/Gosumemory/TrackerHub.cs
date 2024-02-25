using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;
using osu.Framework.Timing;
using osu.Game.Beatmaps;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Overlays;
using osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Data;
using osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Tracker;
using osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Web;
using osu.Game.Scoring;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory
{
    public partial class TrackerHub : CompositeDrawable
    {
        private readonly WebSocketLoader wsLoader;

        public TrackerHub(WebSocketLoader game)
        {
            this.wsLoader = game;
        }

        public DataRoot GetDataRoot()
        {
            return wsLoader.DataRoot;
        }

        private readonly List<AbstractTracker> trackers = new List<AbstractTracker>();

        [BackgroundDependencyLoader]
        private void load()
        {
            Logger.Log("Gosumemoty Compat!");

            this.Clock = new FramedClock(null, false);

            this.addTrackerRange(new AbstractTracker[]
            {
                new BeatmapStrainTracker(this),
                new PPRulesetTracker(this),
                new BeatmapTracker(this),
                new ScreenTracker(this)
            });

#if DEBUG
            AddInternal(new RoundedButton
            {
                Height = 60f,
                Width = 60f,
                Text = "Dump JSON",
                Action = () =>
                {
                    UpdateValues();

                    string str = JsonConvert.SerializeObject(wsLoader.DataRoot, Formatting.None, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Include
                    });

                    Logger.Log(str);
                }
            });
#endif
        }

        private void addTracker(AbstractTracker tracker)
        {
            if (trackers.Contains(tracker)) return;

            trackers.Add(tracker);
            this.AddInternal(tracker);
        }

        private void addTrackerRange(AbstractTracker[] trackers)
        {
            foreach (var abstractTracker in trackers)
                this.addTracker(abstractTracker);
        }

        private void removeTracker(AbstractTracker tracker)
        {
            trackers.Remove(tracker);
            if (this.InternalChildren.Contains(tracker))
                this.RemoveInternal(tracker, true);
        }

        [Resolved]
        private MusicController musicController { get; set; } = null!;

        [Resolved]
        private Bindable<WorkingBeatmap> workingBeatmap { get; set; } = null!;

        [Resolved]
        private OsuGame osuGame { get; set; } = null!;

        private double lastUpdate;

        protected override void Update()
        {
            base.Update();

            //更新太快容易卡住网页
            if (Clock.CurrentTime - lastUpdate < 200) return;

            lastUpdate = Clock.CurrentTime;
            UpdateValues();
        }

        //region InGame and PP

        [Resolved]
        private ScoreManager scoreManager { get; set; } = null!;

        //endregion

        //private CancellationTokenSource? updateCancellationTokenSource;

        public void UpdateValues()
        {
            this.AlwaysPresent = true;

            var obj = wsLoader.DataRoot;
            obj.UpdateTrack(musicController.CurrentTrack);

            foreach (var abstractTracker in trackers)
            {
                try
                {
                    abstractTracker.UpdateValues();
                }
                catch (Exception e)
                {
                    Logging.LogError(e, $"Error occurred while updating tracker {abstractTracker}, disabling this...");
                }
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
    }
}
