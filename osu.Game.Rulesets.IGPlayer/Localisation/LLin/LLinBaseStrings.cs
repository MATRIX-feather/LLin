using osu.Framework.Localisation;

namespace osu.Game.Rulesets.IGPlayer.Localisation.LLin
{
    public static class LLinBaseStrings
    {
        private static string prefix = @"M.Resources.Localisation.LLin.BaseStrings";

        public static LocalisableString Exit => new TranslatableString(getKey(@"exit"), @"Exit");

        public static LocalisableString Manual => new TranslatableString(getKey(@"manual"), @"User manual");

        public static LocalisableString PrevOrRestart => new TranslatableString(getKey(@"prev_restart"), @"Previous / Restart");

        public static LocalisableString Next => new TranslatableString(getKey(@"next"), @"Next");

        public static LocalisableString TogglePause => new TranslatableString(getKey(@"toggle_pause"), @"Toggle pause");

        public static LocalisableString ViewPlugins => new TranslatableString(getKey(@"view_plugins"), @"View plugins");

        public static LocalisableString HideAndLockInterface => new TranslatableString(getKey(@"hide_and_lock_interface"), @"Hide and lock interface");

        public static LocalisableString LockInterface => new TranslatableString(getKey(@"lock_interface"), @"Lock interface");

        public static LocalisableString ToggleLoop => new TranslatableString(getKey(@"toggle_loop"), @"Toggle loop");

        public static LocalisableString ViewInSongSelect => new TranslatableString(getKey(@"view_in_song_select"), @"View in song select");

        public static LocalisableString OpenSidebar => new TranslatableString(getKey(@"open_sidebar"), @"Open sidebar");

        public static LocalisableString AudioControlRequestedMain => new TranslatableString(getKey(@"audio_control_requested_main"), @" requested audio control");

        public static LocalisableString AudioControlRequestedSub(LocalisableString reason) => new TranslatableString(getKey(@"audio_control_requested_sub"), @"Reason: {0}", reason);

        private static string getKey(string key) => $@"{prefix}:{key}";
    }
}
