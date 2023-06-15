namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Data.Consts
{
    public class LegacyGamemodes
    {
        public static readonly int STD = 1;
        public static readonly int MANIA = 2;
        public static readonly int CATCH = 3;
        public static readonly int TAIKO = 4;

        public static int FromRulesetInfo(RulesetInfo info)
        {
            return info.OnlineID;
        }
    }
}
