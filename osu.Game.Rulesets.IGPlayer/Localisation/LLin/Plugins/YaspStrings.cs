using osu.Framework.Localisation;

namespace osu.Game.Rulesets.IGPlayer.Localisation.LLin.Plugins
{
    public static class YaspStrings
    {
        private static string prefix = @"M.Resources.Localisation.LLin.Plugins.YaspStrings";

        public static LocalisableString Scale => new TranslatableString(getKey(@"scale"), @"Classic scaling");

        private static string getKey(string key) => $@"{prefix}:{key}";
    }
}
