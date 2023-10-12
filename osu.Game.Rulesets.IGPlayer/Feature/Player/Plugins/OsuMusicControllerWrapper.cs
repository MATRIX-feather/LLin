using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Audio;
using osu.Framework.Platform;
using osu.Game.Overlays;
using osu.Game.Rulesets.IGPlayer.Player.Graphics.SideBar.Settings.Items;
using osu.Game.Rulesets.IGPlayer.Player.Plugins.Config;
using osu.Game.Rulesets.IGPlayer.Player.Plugins.Types;

namespace osu.Game.Rulesets.IGPlayer.Player.Plugins
{
    internal partial class OsuMusicControllerWrapper : LLinPlugin, IProvideAudioControlPlugin
    {
        [Resolved]
        private MusicController controller { get; set; }

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

        public DrawableTrack GetCurrentTrack() => controller.CurrentTrack;

        private DrawableTrack? prevTrack;

        public Action<DrawableTrack> OnTrackChange { get; set; }

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

        public override IPluginConfigManager CreateConfigManager(Storage storage)
        {
            //workaround: OsuMusicControllerWrapper完成初始化时LLinPluginManager中storage尚未赋值，需要手动获取
            storage ??= (Storage)DependenciesContainer.Get(typeof(Storage));

            return base.CreateConfigManager(storage);
        }

        protected override void Update()
        {
            base.Update();

            var currentTrack = GetCurrentTrack();

            if (currentTrack == prevTrack) return;

            prevTrack = currentTrack;
            OnTrackChange?.Invoke(currentTrack);
        }
    }
}
