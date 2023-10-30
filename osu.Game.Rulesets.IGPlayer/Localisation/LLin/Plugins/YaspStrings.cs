using osu.Framework.Localisation;

namespace osu.Game.Rulesets.IGPlayer.Localisation.LLin.Plugins
{
    public static class YaspStrings
    {
        private const string prefix = @"M.Resources.Localisation.LLin.Plugins.YaspStrings";

        public static LocalisableString Scale => new TranslatableString(getKey(@"scale"), @"Classic scaling");

        public static LocalisableString UseAvatarForCoverIICover => new TranslatableString(getKey(@"use_avatar_for_coverii"), @"(CoverII) Use Avatar");

        private static string getKey(string key) => $@"{prefix}:{key}";
    }
}
