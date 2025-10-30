using System.Collections.Concurrent;
using System.Collections.Generic;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Misc.PluginResolvers
{
    public class LLinPluginResolver(LLinPluginManager pluginManager)
    {
        private readonly LLinPluginManager pluginManager = pluginManager;

        private readonly ConcurrentDictionary<string, LLinPluginProvider> audioPluginDictionary = new();
        private readonly ConcurrentDictionary<string, LLinPluginProvider> functionBarDictionary = new();

        internal void UpdatePluginDictionary(List<LLinPluginProvider> newPluginList)
        {
            functionBarDictionary.Clear();
            audioPluginDictionary.Clear();

            foreach (var provider in newPluginList)
            {
                var plugin = provider.CreatePlugin();
                string id = provider.Identifier();

                switch (plugin)
                {
                    case IFunctionBarProvider:
                        Logging.Log($"Adding function bar {id}");
                        functionBarDictionary[id] = provider;
                        break;

                    case IProvideAudioControlPlugin:
                        Logging.Log($"Adding audio control plugin {id}");
                        audioPluginDictionary[id] = provider;
                        break;
                }
            }
        }

        internal LLinPluginProvider? GetAudioControlPluginByID(string id)
        {
            return audioPluginDictionary.GetValueOrDefault(id);
        }

        internal LLinPluginProvider? GetFunctionBarProviderByID(string id)
        {
            return functionBarDictionary.GetValueOrDefault(id);
        }

        private List<LLinPluginProvider>? cachedAudioControlPluginList;

        internal List<LLinPluginProvider> GetAllAudioControlPlugin()
        {
            var list = new List<LLinPluginProvider>();

            foreach (var keyPair in audioPluginDictionary)
            {
                list.Add(keyPair.Value);
            }

            if (cachedAudioControlPluginList == null || cachedAudioControlPluginList != list)
                cachedAudioControlPluginList = list;

            return list;
        }

        private List<LLinPluginProvider>? cachedFunctionBarPluginList;

        internal List<LLinPluginProvider> GetAllFunctionBarProviders()
        {
            var list = new List<LLinPluginProvider>();

            foreach (var keyPair in functionBarDictionary)
            {
                list.Add(keyPair.Value);
            }

            if (cachedFunctionBarPluginList == null || cachedFunctionBarPluginList != list)
                cachedFunctionBarPluginList = list;

            return list;
        }
    }
}
