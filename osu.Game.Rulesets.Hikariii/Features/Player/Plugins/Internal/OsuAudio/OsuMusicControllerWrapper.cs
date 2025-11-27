using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Overlays;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SideBar.Settings.Items;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Internal.OsuAudio
{
    public partial class OsuMusicControllerWrapper : LLinPlugin, IProvideAudioControlPlugin
    {
        [Resolved]
        private MusicController controller { get; set; } = null!;

        public OsuMusicControllerWrapper(LLinPluginProvider provider)
            : base(provider)
        {
            Name = "osu!";
        }

        public bool NextTrack()
        {
            controller.NextTrack();

            return true;
        }

        public bool PrevTrack()
        {
            controller.PreviousTrack();

            return true;
        }

        public bool TogglePause()
        {
            try
            {
                controller.TogglePause();
            }
            catch (Exception e)
            {
                //播放时此DrawableTrack可能已经被回收
                Logging.LogError(e, "无法播放音频");
                return false;
            }

            return true;
        }

        public bool Seek(double position)
        {
            controller.SeekTo(position);

            return true;
        }

        public bool IsCurrent { get; set; }
        public bool AllowOsuControls { get; } = true;

        protected override Drawable CreateContent() => new PlaceHolder();

        protected override bool OnContentLoaded(Drawable content) => true;

        protected override bool PostInit() => true;
    }
}
