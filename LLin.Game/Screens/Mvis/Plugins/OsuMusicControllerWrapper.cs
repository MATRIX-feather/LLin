using System;
using LLin.Game.Screens.Mvis.Plugins.Types;
using LLin.Game.Screens.Mvis.SideBar.Settings.Items;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Audio;
using osu.Game.Overlays;

namespace LLin.Game.Screens.Mvis.Plugins
{
    internal class OsuMusicControllerWrapper : MvisPlugin, IProvideAudioControlPlugin
    {
        [Resolved]
        private MusicController controller { get; set; }

        public void NextTrack() => controller.NextTrack();

        public void PrevTrack() => controller.PreviousTrack();

        public void TogglePause() => controller.TogglePause();

        public void Seek(double position) => controller.SeekTo(position);

        public DrawableTrack GetCurrentTrack()
        {
            if (controller != null)
                return controller.CurrentTrack;

            throw new InvalidOperationException("controller是null");
        }

        public bool IsCurrent { get; set; }

        protected override Drawable CreateContent() => new PlaceHolder();

        protected override bool OnContentLoaded(Drawable content) => true;

        protected override bool PostInit() => true;

        public override int Version => 1;

        public OsuMusicControllerWrapper()
        {
            Name = "osu!";
            Description = "osu!音乐兼容插件";
            Author = "mf-osu";
        }
    }
}
