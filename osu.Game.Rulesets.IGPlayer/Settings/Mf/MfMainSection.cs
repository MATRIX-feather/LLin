using osu.Framework.Localisation;
using osu.Game.Overlays.Settings;

namespace osu.Game.Rulesets.IGPlayer.Settings.Mf
{
    public sealed partial class MfMainSection : RulesetSettingsSubsection
    {
        protected override LocalisableString Header { get; } = "Hikariii";

        public MfMainSection(Ruleset ruleset)
            : base(ruleset)
        {
            Add(new MfMvisPluginSection());
            //Add(new LinuxSection());
        }
    }
}
