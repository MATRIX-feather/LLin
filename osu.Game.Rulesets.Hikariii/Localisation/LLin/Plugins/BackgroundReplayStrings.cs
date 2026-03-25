using osu.Framework.Localisation;

namespace osu.Game.Rulesets.Hikariii.Localisation.LLin.Plugins
{
    public class BackgroundReplayStrings
    {
        private const string prefix = "M.Resources.Localisation.LLin.Plugins.BackgroundReplayStrings";

        // 设置
        public static LocalisableString AutoplayPreference => new TranslatableString(getKey(@"autoplay_preference"), @"Use Autoplay replays");

        public static LocalisableString OnlyUsePassedScores => new TranslatableString(getKey(@"only_passed_scores"), "Only use passed scores");

        // Autoplay 回放偏好
        public static LocalisableString AlwaysUseAutoplay => new TranslatableString(getKey(@"autoplay_always"), @"Always");

        public static LocalisableString UseAutoplayAsAlternative => new TranslatableString(getKey(@"autoplay_as_alternative"), @"When no replay is found");

        public static LocalisableString NeverUseAutoplay => new TranslatableString(getKey(@"autoplay_never"), @"Never");

        private static string getKey(string key) => $@"{prefix}:{key}";
    }
}
