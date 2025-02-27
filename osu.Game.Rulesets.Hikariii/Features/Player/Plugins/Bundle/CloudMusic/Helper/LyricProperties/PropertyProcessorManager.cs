using System;
using System.Collections.Generic;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.CloudMusic.Helper.LyricProperties.Processors;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.CloudMusic.Misc;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.CloudMusic.Helper.LyricProperties
{
    public class PropertyProcessorManager
    {
        public static readonly PropertyProcessorManager INSTANCE = new PropertyProcessorManager();

        private readonly List<IPropertyProcessor> processors = [];

        private PropertyProcessorManager()
        {
            processors.AddRange(
            [
                new TimeProcessor(),
                new AuthorProcessor()
            ]);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="propertyInput"></param>
        /// <param name="lyric"></param>
        /// <returns>至少有一个属性处理器处理成功</returns>
        public bool Process(string propertyInput, Lyric lyric)
        {
            bool atLeastOneProcessed = false;

            foreach (var propertyProcessor in processors)
            {
                try
                {
                    atLeastOneProcessed = atLeastOneProcessed || propertyProcessor.Process(propertyInput, lyric);
                }
                catch (Exception e)
                {
                    Logging.LogError(e, $"{propertyProcessor.GetType()} Failed to process property {propertyInput}: {e.Message}");
                }
            }

            return atLeastOneProcessed;
        }
    }
}
