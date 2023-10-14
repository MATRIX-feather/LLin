using Humanizer;
using osu.Framework.Localisation;

namespace osu.Game.Rulesets.IGPlayer.Localisation.LLin.Plugins
{
    public static class CollectionStrings
    {
        private const string prefix = "M.Resources.Localisation.LLin.Plugins.CollectionStrings";

        public static LocalisableString NoCollectionSelected => new TranslatableString(getKey(@"no_collection_selected"), @"No Collection Selected");

        public static LocalisableString SelectOneFirst => new TranslatableString(getKey(@"select_one_first"), @"Select One First!");

        public static LocalisableString AudioControlRequest => new TranslatableString(getKey(@"audio_control_request"), "Activate to ensure the plugin can function properly\nThis prompt won't show again in this session");

        public static LocalisableString EntryTooltip => new TranslatableString(getKey(@"entry_tooltip"), "Browse Collections");

        public static LocalisableString SongCount(int count) => new TranslatableString(getKey(@"song_count"), @"Song".ToQuantity(count), count);

        private static string getKey(string key) => $@"{prefix}:{key}";
    }
}
