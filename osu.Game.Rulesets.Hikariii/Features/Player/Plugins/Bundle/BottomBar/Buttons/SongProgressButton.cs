using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Audio;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.BottomBar.Buttons
{
    public partial class SongProgressButton : BottomBarSwitchButton
    {
        private string? timeCurrent;
        private string? timeTotal;

        [Resolved]
        private IImplementLLin mvis { get; set; } = null!;

        private DrawableTrack track => mvis.CurrentTrack;

        private string formatTime(TimeSpan timeSpan) => $"{(timeSpan < TimeSpan.Zero ? "-" : "")}{Math.Floor(timeSpan.Duration().TotalMinutes)}:{timeSpan.Duration().Seconds:D2}";

        public SongProgressButton(IToggleableFunctionProvider provider)
            : base(provider)
        {
            AutoSizeAxes = Axes.X;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            mvis.OnBeatmapChanged(b => lastSecond = -1, this, true);

            if (this.outerContent != null)
            {
                this.outerContent.RelativeSizeAxes = Axes.Y;
                this.outerContent.AutoSizeAxes = Axes.X;
            }
        }

        private int lastSecond;

        protected override void Update()
        {
            base.Update();

            int currentSecond = (int)Math.Floor(track.CurrentTime / 1000.0);
            if (lastSecond == currentSecond) return;

            lastSecond = currentSecond;
            timeCurrent = formatTime(TimeSpan.FromSeconds(currentSecond));
            timeTotal = formatTime(TimeSpan.FromMilliseconds(track.Length));
            Title = $"{timeCurrent} / {timeTotal}";
        }
    }
}
