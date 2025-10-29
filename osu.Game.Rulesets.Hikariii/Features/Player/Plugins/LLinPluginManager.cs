using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using osu.Framework;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Configuration;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SettingsItems;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Misc;
using osu.Game.Rulesets.Hikariii.Features.Player.Misc.PluginResolvers;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.BottomBar;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.CloudMusic;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.SandboxToPanel;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Storyboard;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Yasp;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Internal.DummyAudio;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Internal.DummyBase;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Internal.FallbackFunctionBar;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins
{
    public partial class LLinPluginManager : CompositeDrawable
    {
        #region 插件管理

        private readonly Dictionary<string, LLinPluginProvider> providerMap = new();

        private readonly BindableList<LLinPlugin> avaliablePlugins = new BindableList<LLinPlugin>();
        private readonly BindableList<LLinPlugin> activePlugins = new BindableList<LLinPlugin>();

        private readonly LLinPluginResolver resolver;

        private string blockedPluginFilePath => storage.GetFullPath("custom/blocked_plugins.json");

        #endregion

        #region 插件配置

        private readonly ConcurrentDictionary<Type, IPluginConfigManager> configManagers = new ConcurrentDictionary<Type, IPluginConfigManager>();
        private readonly ConcurrentDictionary<Type, SettingsEntry[]> entryMap = new ConcurrentDictionary<Type, SettingsEntry[]>();

        #endregion

        #region 依赖

        [Resolved]
        private Storage storage { get; set; } = null!;

        public readonly IProvideAudioControlPlugin DefaultAudioController = new OsuMusicControllerWrapper();

        public readonly TypeWrapper DefaultFunctionBarType = new TypeWrapper
        {
            Type = typeof(FunctionBar),
            Name = "默认底栏"
        };

        public readonly TypeWrapper DefaultAudioControllerType = new TypeWrapper
        {
            Type = typeof(OsuMusicControllerWrapper),
            Name = "osu!"
        };

        #endregion

        #region 内部方法/参数

        internal Action<LLinPlugin>? OnPluginAdd;
        internal Action<LLinPlugin>? OnPluginUnLoad;

        internal static int LatestPluginVersion => 10;

        internal SettingsEntry[]? GetSettingsFor(LLinPlugin pl)
        {
            if (!entryMap.ContainsKey(pl.GetType()))
                Logging.Log($"entryMap中没有和{pl}有关的数据。");

            return entryMap.ContainsKey(pl.GetType()) ? entryMap[pl.GetType()] : null;
        }

        internal List<TypeWrapper> GetAllFunctionBarProviders() => resolver.GetAllFunctionBarProviders();

        internal List<TypeWrapper> GetAllAudioControlPlugin() => resolver.GetAllAudioControlPlugin();

        internal Type? GetAudioControlTypeByPath([NotNull] string path) => resolver.GetAudioControlPluginByPath(path);
        internal Type? GetFunctionBarProviderTypeByPath([NotNull] string path) => resolver.GetFunctionBarProviderByPath(path);

        internal IProvideAudioControlPlugin? GetAudioControlByPath([NotNull] string path)
            => (IProvideAudioControlPlugin?)avaliablePlugins.FirstOrDefault(pl => pl is IProvideAudioControlPlugin && resolver.ToPath(pl) == path);

        internal IFunctionBarProvider? GetFunctionBarProviderByPath([NotNull] string path)
            => (IFunctionBarProvider?)avaliablePlugins.FirstOrDefault(pl => pl is IFunctionBarProvider && resolver.ToPath(pl) == path);

        private bool platformSupportsDBus => RuntimeInfo.OS == RuntimeInfo.Platform.Linux && (FeatureManager.Instance?.CanUseDBus.Value ?? false);

        public X? AcquireProvider<X>(string identifier)
            where X : LLinPluginProvider
        {
            var provider = providerMap!.GetValueOrDefault(identifier, null);

            if (provider is X x)
                return x;

            return null;
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

        internal bool AddPlugin(LLinPlugin? pl)
        {
            if (pl == null || avaliablePlugins.Contains(pl)) return false;

            if (pl.Version < MinimumPluginVersion)
                Logging.Log($"插件 \"{pl.Name}\" 是为旧版本的mf-osu打造的, 继续使用可能会导致意外情况的发生!", LoggingTarget.Runtime, LogLevel.Important);
            else if (pl.Version > PluginVersion)
                Logging.Log($"插件 \"{pl.Name}\" 是为更高版本的mf-osu打造的, 继续使用可能会导致意外情况的发生!", LoggingTarget.Runtime, LogLevel.Important);

            avaliablePlugins.Add(pl);
            OnPluginAdd?.Invoke(pl);

            pl.PluginManager = this;
            return true;
        }

        internal bool ActivePlugin(LLinPlugin pl)
        {
            if (!avaliablePlugins.Contains(pl) || activePlugins.Contains(pl) || pl == null) return false;

            if (!activePlugins.Contains(pl))
                activePlugins.Add(pl);

            bool success = pl.Enable();

            if (!success)
                activePlugins.Remove(pl);

            return success;
        }

        internal bool DisablePlugin(LLinPlugin pl)
        {
            if (!avaliablePlugins.Contains(pl) || !activePlugins.Contains(pl) || pl == null) return false;

            activePlugins.Remove(pl);
            bool success = pl.Disable();

            if (!success)
            {
                activePlugins.Add(pl);
                Logging.Log($"卸载插件\"${pl.Name}\"失败");
            }

            return success;
        }

        internal void ExpireOldPlugins()
        {
            foreach (var pl in avaliablePlugins)
            {
                activePlugins.Remove(pl);
                pl.Expire();
            }

            avaliablePlugins.Clear();
        }

        #endregion

        #region API相关

        public int PluginVersion => LatestPluginVersion;
        public int MinimumPluginVersion => 9;

        public IPluginConfigManager GetConfigManager(LLinPlugin pl) =>
            configManagers.GetOrAdd(pl.GetType(), _ => pl.CreateConfigManager(storage));

        public List<LLinPlugin> GetActivePlugins() => activePlugins.ToList();

        /// <summary>
        /// 获取所有插件
        /// </summary>
        /// <param name="newInstance">
        /// 是否处理当前所有插件并创建新插件本体<br/>
        /// </param>
        /// <returns>所有已加载且可用的插件</returns>
        public List<LLinPlugin> GetAllPlugins(bool newInstance)
        {
            if (newInstance)
            {
                //bug: 直接调用Dispose会导致快速进出时抛出Disposed drawabled may never in the scene graph
                ExpireOldPlugins();

                foreach (var p in providerMap.Values)
                {
                    avaliablePlugins.Add(p.CreatePlugin);
                }

                resolver.UpdatePluginDictionary(avaliablePlugins.ToList());
            }

            return avaliablePlugins.ToList();
        }

        #endregion

        internal PluginStore? PluginStore;

        public LLinPluginManager()
        {
            resolver = new LLinPluginResolver(this);

            InternalChild = (OsuMusicControllerWrapper)DefaultAudioController;
        }

        [BackgroundDependencyLoader]
        private void load(OsuGameBase gameBase, MConfigManager config)
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

            // 内置插件
            DummyBasePluginProvider dbpp;
            DummyAudioPluginProvider dapp;
            RegisterProvider(dbpp = new DummyBasePluginProvider(config, this));
            RegisterProvider(dapp = new DummyAudioPluginProvider(config, this));

            AddPlugin(dbpp.CreatePlugin);
            AddPlugin(dapp.CreatePlugin);
            //AddPlugin(luapp.CreatePlugin);

            // 随Ruleset附送
            var bundledPlugins = new LLinPluginProvider[]
            {
                new SandboxPanelProvider(),
                new BottomBarProvider(),
                new CollectionHelperProvider(),
                new StoryboardPluginProvider(),

                new LyricPluginProvider(),
                new YaspProvider(),

                //new NewBottomBarProvider()
            };

            bundledPlugins.ForEach(p => RegisterProvider(p));

            foreach (var lLinPluginProvider in bundledPlugins)
                AddPlugin(lLinPluginProvider.CreatePlugin);

            if (PluginStore != null)
            {
                foreach (LLinPluginProvider provider in PluginStore.LoadedPluginProviders)
                {
                    RegisterProvider(provider);
                    AddPlugin(provider.CreatePlugin);
                }
            }

            resolver.UpdatePluginDictionary(GetAllPlugins(false));

            foreach (var pl in this.GetAllPlugins(false))
            {
                try
                {
                    var oldEntries = pl.GetSettingEntries();

                    if (oldEntries != null)
                        entryMap[pl.GetType()] = oldEntries;
                    else
                        entryMap[pl.GetType()] = pl.GetSettingEntries(GetConfigManager(pl));
                }
                catch (Exception e)
                {
                    Logging.Log($"Unable to update settings for plugin {pl}");
                    Logging.Log(e.Message);
                    Logging.Log(e.StackTrace);
                }
            }
        }

        public string ToPath([NotNull] object target) => resolver.ToPath(target);
    }
}
