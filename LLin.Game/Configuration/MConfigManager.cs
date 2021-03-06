using System;
using System.ComponentModel;
using LLin.Game.Screens.Mvis.SideBar.Tabs;
using osu.Framework;
using osu.Framework.Configuration;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osuTK.Graphics;

namespace LLin.Game.Configuration
{
    public class MConfigManager : IniConfigManager<MSetting>
    {
        protected override string Filename => "mf.ini";

        public MConfigManager(Storage storage)
            : base(storage)
        {
        }

        protected override void InitialiseDefaults()
        {
            base.InitialiseDefaults();

            //Other Settings
            SetDefault(MSetting.UseSayobot, true);
            SetDefault(MSetting.DoNotShowDisclaimer, false);

            //UI Settings
            SetDefault(MSetting.OptUI, true);
            SetDefault(MSetting.TrianglesEnabled, true);
            SetDefault(MSetting.SongSelectBgBlur, 0.2f, 0f, 1f);
            SetDefault(MSetting.AlwaysHideTextIndicator, false);

            //Intro Settings
            SetDefault(MSetting.IntroLoadDirectToSongSelect, false);

            //Gameplay Settings
            SetDefault(MSetting.SamplePlaybackGain, 1f, 0f, 20f);

            //MvisSettings
            SetDefault(MSetting.MvisContentAlpha, 1f, 0f, 1f);
            SetDefault(MSetting.MvisBgBlur, 0.2f, 0f, 1f);
            SetDefault(MSetting.MvisStoryboardProxy, false);
            SetDefault(MSetting.MvisIdleBgDim, 0.8f, 0f, 1f);
            SetDefault(MSetting.MvisEnableBgTriangles, true);
            SetDefault(MSetting.MvisAdjustMusicWithFreq, true);
            SetDefault(MSetting.MvisMusicSpeed, 1.0, 0.1, 2.0);
            SetDefault(MSetting.MvisEnableNightcoreBeat, false);
            SetDefault(MSetting.MvisPlayFromCollection, false);
            SetDefault(MSetting.MvisInterfaceRed, value: 0, 0, 255f);
            SetDefault(MSetting.MvisInterfaceGreen, value: 119f, 0, 255f);
            SetDefault(MSetting.MvisInterfaceBlue, value: 255f, 0, 255f);
            SetDefault(MSetting.MvisCurrentAudioProvider, "a@b");
            SetDefault(MSetting.MvisCurrentFunctionBar, "LegacyBottomBar@Mvis.Plugin.BottomBar");
            SetDefault(MSetting.MvisTabControlPosition, TabControlPosition.Right);

            //???????????????
            SetDefault(MSetting.CustomWindowIconPath, "");
            SetDefault(MSetting.UseCustomGreetingPicture, false);
            SetDefault(MSetting.FadeOutWindowWhenExiting, false);
            SetDefault(MSetting.FadeInWindowWhenEntering, false);
            SetDefault(MSetting.UseSystemCursor, false);
            SetDefault(MSetting.PreferredFont, "Torus");
            SetDefault(MSetting.LoaderBackgroundColor, "#000000");

            //Gamemode??????
            SetDefault(MSetting.Gamemode, GamemodeActivateCondition.InGame);

            var isLinuxPlatform = RuntimeInfo.OS == RuntimeInfo.Platform.Linux;

            //DBus??????
            SetDefault(MSetting.DBusIntegration, isLinuxPlatform);
            SetDefault(MSetting.DBusAllowPost, true);
            SetDefault(MSetting.EnableTray, isLinuxPlatform);
            SetDefault(MSetting.EnableSystemNotifications, isLinuxPlatform);

            //Mpris
            SetDefault(MSetting.MprisUseAvatarlogoAsCover, true);
        }

        public Color4 GetCustomLoaderColor()
        {
            try
            {
                if (Get<bool>(MSetting.UseCustomGreetingPicture))
                    return Color4Extensions.FromHex(Get<string>(MSetting.LoaderBackgroundColor));
                else
                    return Color4.Black;
            }
            catch (Exception e)
            {
                SetValue(MSetting.LoaderBackgroundColor, "#000000");
                Logger.Error(e, "??????????????????????????????, ????????????????????????");
                return Color4.Black;
            }
        }
    }

    public enum MSetting
    {
        OptUI,
        TrianglesEnabled,
        UseSayobot,
        MvisBgBlur,
        MvisStoryboardProxy,
        MvisIdleBgDim,
        MvisContentAlpha,
        MvisEnableBgTriangles,
        MvisMusicSpeed,
        MvisAdjustMusicWithFreq,
        MvisEnableNightcoreBeat,
        MvisPlayFromCollection,
        MvisInterfaceRed,
        MvisInterfaceGreen,
        MvisInterfaceBlue,
        MvisTabControlPosition,
        SamplePlaybackGain,
        SongSelectBgBlur,
        IntroLoadDirectToSongSelect,
        CustomWindowIconPath,
        UseCustomGreetingPicture,
        FadeOutWindowWhenExiting,
        FadeInWindowWhenEntering,
        UseSystemCursor,
        PreferredFont,
        AlwaysHideTextIndicator,
        MvisCurrentAudioProvider,
        Gamemode,
        DoNotShowDisclaimer,
        LoaderBackgroundColor,
        MvisCurrentFunctionBar,
        DBusIntegration,
        DBusAllowPost,
        MprisUseAvatarlogoAsCover,
        EnableTray,
        EnableSystemNotifications
    }

    public enum GamemodeActivateCondition
    {
        [Description("??????")]
        Never,

        [Description("????????????")]
        InGame,

        [Description("??????")]
        Always
    }
}
