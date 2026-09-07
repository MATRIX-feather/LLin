using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics.Containers;
using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Configuration;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SettingsItems;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Misc.PluginResolvers;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.BottomBar;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.CloudMusic;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Replay;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.SandboxToPanel;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Storyboard;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Internal.FallbackFunctionBar;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Internal.OsuAudio;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins
{
    [Obsolete("Gonna be removed in future")]
    public partial class LLinPluginManager : CompositeDrawable
    {
        #region 插件管理

        private readonly Dictionary<string, LLinPluginProvider> providerMap = new();
        private readonly LLinPluginResolver resolver;

        #endregion

        #region 插件配置

        private readonly ConcurrentDictionary<string, IPluginConfigManager> configManagers = new();
        private readonly ConcurrentDictionary<string, SettingsEntry[]> entryMap = new();

        #endregion

        #region 依赖

        [Resolved]
        private Storage storage { get; set; } = null!;

        public OsuMusicControllerWrapper AcquireOsuAudioController()
        {
            throw new NotImplementedException();
            //return (OsuMusicControllerWrapper)AcquireProviderOrThrow<OsuAudioPluginProvider>(OsuAudioPluginProvider.ID).CreatePlugin();
        }

        #endregion

        #region 内部方法/参数

        internal static int LatestPluginVersion => 10;

        public SettingsEntry[] GetSettingsFor(string id) => entryMap.GetValueOrDefault(id, []);
        public SettingsEntry[] GetSettingsFor(LLinPluginProvider provider) => GetSettingsFor(provider.Identifier());

        internal List<LLinPluginProvider> GetAllFunctionBarProviders() => resolver.GetAllFunctionBarProviders();

        internal List<LLinPluginProvider> GetAllAudioControlPlugin() => resolver.GetAllAudioControlPlugin();

        internal IProvideAudioControlPlugin? GetAudioControlByID(string id)
            => (IProvideAudioControlPlugin?)resolver.GetAudioControlPluginByID(id)?.CreatePlugin();

        internal IFunctionBarProvider? GetFunctionBarProviderByID(string id)
            => (IFunctionBarProvider?)resolver.GetFunctionBarProviderByID(id)?.CreatePlugin();

        public X? AcquireProvider<X>(string identifier)
            where X : LLinPluginProvider
        {
            var provider = providerMap!.GetValueOrDefault(identifier, null);

            if (provider is X x)
                return x;

            return null;
        }

        public X AcquireProviderOrThrow<X>(string identifier)
            where X : LLinPluginProvider
        {
            var result = AcquireProvider<X>(identifier);
            if (result == null)
                throw new Exception($"Expected an instance of {identifier}, but got null.");

            return result;
        }

        public bool RegisterProvider(LLinPluginProvider provider)
        {
            if (providerMap.ContainsKey(provider.Identifier()))
            {
                Logging.Log($"We already have a provider with the same identifier '{provider.Identifier()}'!");
                return false;
            }

            providerMap[provider.Identifier()] = provider;
            return true;
        }

        #endregion

        #region API相关

        public int PluginVersion => LatestPluginVersion;
        public int MinimumPluginVersion => 9;

        public IPluginConfigManager GetConfigManager(string id)
        {
            var provider = providerMap!.GetValueOrDefault(id, null);
            return provider == null
                ? throw new Exception("The given ID doesn't match any providers")
                : configManagers.GetOrAdd(id, _ => provider.CreateConfigManager(storage));
        }

        public IPluginConfigManager GetConfigManager(LLinPluginProvider provider) => GetConfigManager(provider.Identifier());

        public Dictionary<string, LLinPluginProvider> GetAllPluginProviders()
        {
            return new Dictionary<string, LLinPluginProvider>(providerMap);
        }

        #endregion

        internal PluginStore? PluginStore;

        public LLinPluginManager()
        {
            //resolver = new LLinPluginResolver(this);
        }

        [BackgroundDependencyLoader]
        private void load(OsuGameBase gameBase, DeprecatedMConfigManager config)
        {
            // Uncomment if we ever want blocking plugins feature back again
            /*
            try
            {
                if (!File.Exists(blockedPluginFilePath))
                    File.Create(blockedPluginFilePath);

                using var writer = new StreamReader(File.OpenRead(blockedPluginFilePath));

                var obj = JsonConvert.DeserializeObject<List<string>>(writer.ReadToEnd());

                if (obj != null)
                    blockedProviders.AddRange(obj);
            }
            catch (Exception e)
            {
                if (e is not FileNotFoundException)
                    Logging.LogError(e, "读取黑名单插件列表时出现了问题");
            }*/

            try
            {
                PluginStore = new PluginStore(storage, gameBase);
            }
            catch (Exception e)
            {
                Logging.LogError(e, $"未能初始化插件存储, 本次启动将不会加载任何外部插件！({e.Message})");
                PluginStore = null;
            }

            // 随Ruleset附送
            var bundledPlugins = new LLinPluginProvider[]
            {
                new FallbackFunctionBarProvider(),

                new SandboxPanelProvider(),
                new StandardBottomBarProvider(),
                new CollectionHelperProvider(),
                new StoryboardPluginProvider(),

                new LyricPluginProvider(),

                new ReplayPluginProvider()

                //new NewBottomBarProvider()
            };

            bundledPlugins.ForEach(p => RegisterProvider(p));

            if (PluginStore != null)
            {
                foreach (LLinPluginProvider provider in PluginStore.LoadedPluginProviders)
                    RegisterProvider(provider);
            }

            var providers = GetAllPluginProviders().Values.ToList();

            resolver.UpdatePluginDictionary(providers);

            foreach (var pl in providers)
            {
                try
                {
                    var pluginConfigManager = GetConfigManager(pl.Identifier());
                    pl.EarlyInitSettingEntries(pluginConfigManager);
                    entryMap[pl.Identifier()] = pl.GetSettingEntries(pluginConfigManager);
                }
                catch (Exception e)
                {
                    Logging.Log($"Unable to update settings for plugin {pl}");
                    Logging.Log(e.Message);
                    Logging.Log(e.StackTrace);
                }
            }
        }
    }
}
