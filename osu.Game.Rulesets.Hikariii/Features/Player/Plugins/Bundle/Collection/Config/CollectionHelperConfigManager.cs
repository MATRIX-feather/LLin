using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection.Sorter;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Config;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection.Config
{
    public class CollectionHelperConfigManager : PluginConfigManager<CollectionSettings>
    {
        public CollectionHelperConfigManager(Storage storage)
            : base(storage, null) //todo: FIXME fix null provider
        {
        }

        protected override void InitialiseDefaults()
        {
            SetDefault(CollectionSettings.EnablePlugin, false);
            SetDefault(CollectionSettings.EnableRandom, false);
            SetDefault(CollectionSettings.BeatmapSortMethod, SortMethod.MostDifficultFirst);
            base.InitialiseDefaults();
        }
    }

    public enum CollectionSettings
    {
        EnablePlugin,
        EnableRandom,
        BeatmapSortMethod
    }
}
