using osu.Framework.Localisation;

namespace osu.Game.Rulesets.IGPlayer.Localisation.LLin.Plugins
{
    public class CloudMusicStrings
    {
        private const string prefix = "M.Resources.Localisation.LLin.Plugins.CloudMusicStrings";

        //设置
        public static LocalisableString LocationDirection => new TranslatableString(getKey(@"location_direction"), @"Location Direction");

        public static LocalisableString PositionX => new TranslatableString(getKey(@"pos_x"), @"X Position");

        public static LocalisableString PositionY => new TranslatableString(getKey(@"pos_y"), @"Y Position");

        public static LocalisableString UseDrawablePool => new TranslatableString(getKey(@"use_drawable_pool"), @"Use DrawablePool");

        public static LocalisableString ExperimentalWarning => new TranslatableString(getKey(@"experimental_warning"), @"Experimental Feature!");

        public static LocalisableString SaveLyricOnDownloadedMain => new TranslatableString(getKey(@"save_lrc_on_downloaded_main"), @"Save Lyrics");

        public static LocalisableString SaveLyricOnDownloadedSub => new TranslatableString(getKey(@"save_lrc_on_downloaded_sub"), "Lyrics will be saved in custom/lyrics/beatmap-<ID>.json");

        public static LocalisableString DisableShader => new TranslatableString(getKey(@"disable_shader"), @"Disable Shadows");

        public static LocalisableString LocalOffset => new TranslatableString(getKey(@"global_offset_main"), @"Local Offset");

        public static LocalisableString LyricFadeInDuration => new TranslatableString(getKey(@"lyric_fade_in_duration"), @"Fade-In Duration");

        public static LocalisableString LyricFadeOutDuration => new TranslatableString(getKey(@"lyric_fade_out_duration"), @"Fade-Out Duration");

        public static LocalisableString LyricAutoScrollMain => new TranslatableString(getKey(@"lyric_auto_scroll_main"), @"Auto Scroll");

        public static LocalisableString LyricAutoScrollSub => new TranslatableString(getKey(@"lyric_auto_scroll_sub"), @"Enables auto scrolling in lyric interface");

        public static LocalisableString AudioControlRequest => new TranslatableString(getKey(@"audio_control_request"), @"Editing lyrics requires disabling song switching");

        public static LocalisableString EntryTooltip => new TranslatableString(getKey(@"entry_tooltip"), @"Open Lyrics Panel");

        public static LocalisableString Refresh => new TranslatableString(getKey(@"refresh"), @"Refresh");

        public static LocalisableString RefetchLyric => new TranslatableString(getKey(@"refetch_lyric"), @"Re-fetch Lyrics");

        public static LocalisableString ScrollToCurrent => new TranslatableString(getKey(@"scroll_to_current"), @"Scroll to Current Lyrics");

        //编辑屏幕相关
        public static LocalisableString Edit => new TranslatableString(getKey(@"edit"), @"Edit");

        public static LocalisableString Save => new TranslatableString(getKey(@"save"), @"Save");

        public static LocalisableString Reset => new TranslatableString(getKey(@"reset"), @"Reset");

        public static LocalisableString Delete => new TranslatableString(getKey(@"delete"), @"Delete");

        public static LocalisableString SeekToNext => new TranslatableString(getKey(@"seek_next"), @"Seek to Next Beat");

        public static LocalisableString SeekToPrev => new TranslatableString(getKey(@"seek_prev"), @"Seek to Previous Beat");

        public static LocalisableString InsertNewLine => new TranslatableString(getKey(@"insert_new_line"), @"Insert New Line");

        public static LocalisableString LyricTime => new TranslatableString(getKey(@"lyric_time"), @"Lyric Time (ms)");

        public static LocalisableString LyricRaw => new TranslatableString(getKey(@"raw_lyric"), @"Lyric Text");

        public static LocalisableString LyricTranslated => new TranslatableString(getKey(@"lyric_translated"), @"Translated Lyrics");

        public static LocalisableString LyricTimeToTrack => new TranslatableString(getKey(@"lyric_time_to_track"), @"Adjust Lyrics to Song Time");

        public static LocalisableString TrackTimeToLyric => new TranslatableString(getKey(@"track_time_to_lyric"), @"Adjust Song Time to Lyrics");

        //其他
        public static LocalisableString AdjustOffsetToLyric => new TranslatableString(getKey(@"offset_adjust_to_lyric"), @"Adjust offset to lyric");

        private static string getKey(string key) => $@"{prefix}:{key}";
    }
}
