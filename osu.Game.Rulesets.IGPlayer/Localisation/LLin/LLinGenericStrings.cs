using osu.Framework.Localisation;

namespace osu.Game.Rulesets.IGPlayer.Localisation.LLin
{
    public static class LLinGenericStrings
    {
        private const string prefix = @"M.Resources.Localisation.LLin.GenericStrings";

        public static LocalisableString EnablePlugin => new TranslatableString(getKey(@"enable_plugin"), @"Enable Plugin");

        public static LocalisableString DisablePlugin => new TranslatableString(getKey(@"disable_plugin"), @"Disable Plugin");

        private static string getKey(string key) => $@"{prefix}:{key}";
    }
}
