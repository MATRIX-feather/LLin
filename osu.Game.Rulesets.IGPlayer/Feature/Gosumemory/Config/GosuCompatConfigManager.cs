using System.Collections.Generic;
using osu.Framework.Configuration;
using osu.Framework.Platform;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Config
{
    public class GosuCompatConfigManager : IniConfigManager<GosuSettings>
    {
        protected override string Filename => "glazer.ini";

        public GosuCompatConfigManager(Storage storage, IDictionary<GosuSettings, object>? defaultOverrides = null)
            : base(storage, defaultOverrides)
        {
        }

        protected override void InitialiseDefaults()
        {
            base.InitialiseDefaults();

            SetDefault(GosuSettings.UpdateInterval, 200, 50, 1000);
            SetDefault(GosuSettings.MaximumSegments, 512, 64, 1024);
            SetDefault(GosuSettings.SegmentScale, 1, 0.1f, 2f);
        }
    }

    public enum GosuSettings
    {
        UpdateInterval,
        MaximumSegments,
        SegmentScale
    }
}
