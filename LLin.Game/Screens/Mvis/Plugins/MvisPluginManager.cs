using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using LLin.Game.Screens.Mvis.Misc.PluginResolvers;
using LLin.Game.Screens.Mvis.Plugins.Config;
using LLin.Game.Screens.Mvis.Plugins.Internal;
using LLin.Game.Screens.Mvis.Plugins.Types;
using M.DBus;
using M.DBus.Services.Notifications;
using M.DBus.Tray;
using Newtonsoft.Json;
using osu.Framework;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Development;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;
using osu.Framework.Platform;
using Tmds.DBus;

namespace LLin.Game.Screens.Mvis.Plugins
{
    public class MvisPluginManager : CompositeDrawable
    {
        private readonly BindableList<MvisPlugin> avaliablePlugins = new BindableList<MvisPlugin>();
        private readonly BindableList<MvisPlugin> activePlugins = new BindableList<MvisPlugin>();
        private readonly List<MvisPluginProvider> providers = new List<MvisPluginProvider>();
        private List<string> blockedProviders;

        private readonly ConcurrentDictionary<Type, IPluginConfigManager> configManagers = new ConcurrentDictionary<Type, IPluginConfigManager>();

        [Resolved]
        private Storage storage { get; set; }

        [Resolved]
        private CustomStore customStore { get; set; }

        [Resolved]
        private DBusManager dBusManager { get; set; }

        internal Action<MvisPlugin> OnPluginAdd;
        internal Action<MvisPlugin> OnPluginUnLoad;

        public int PluginVersion => 7;
        public int MinimumPluginVersion => 6;
        private const bool experimental = true;

        public readonly IProvideAudioControlPlugin DefaultAudioController = new OsuMusicControllerWrapper();
        public readonly IFunctionBarProvider DummyFunctionBar = new DummyFunctionBar();

        private readonly MvisPluginResolver resolver;

        private string blockedPluginFilePath => storage.GetFullPath("custom/blocked_plugins.json");

        public MvisPluginManager()
        {
            resolver = new MvisPluginResolver(this);

            InternalChild = (OsuMusicControllerWrapper)DefaultAudioController;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            try
            {
                using (var writer = new StreamReader(File.OpenRead(blockedPluginFilePath)))
                {
                    blockedProviders = JsonConvert.DeserializeObject<List<string>>(writer.ReadToEnd())
                                       ?? new List<string>();
                }
            }
            catch (Exception e)
            {
                if (!(e is FileNotFoundException))
                    Logger.Error(e, "?????????????????????????????????????????????");

                blockedProviders = new List<string>();
            }

            foreach (var provider in customStore.LoadedPluginProviders)
            {
                if (!blockedProviders.Contains(provider.GetType().Assembly.ToString()))
                {
                    AddPlugin(provider.CreatePlugin);
                    providers.Add(provider);
                }
            }

            resolver.UpdatePluginDictionary(GetAllPlugins(false));

            if (!DebugUtils.IsDebugBuild && experimental)
            {
                Logger.Log($"?????????????????? ({PluginVersion}) ??????????????????????????? "
                           + "???????????????????????????????????????????????????????????? ",
                    LoggingTarget.Runtime,
                    LogLevel.Important);
            }
        }

        public IPluginConfigManager GetConfigManager(MvisPlugin pl) =>
            configManagers.GetOrAdd(pl.GetType(), _ => pl.CreateConfigManager(storage));

        public void RegisterDBusObject(IDBusObject target)
        {
            if (platformSupportsDBus)
                dBusManager.RegisterNewObject(target);
        }

        public void UnRegisterDBusObject(IDBusObject target)
        {
            if (platformSupportsDBus)
                dBusManager.UnRegisterObject(target);
        }

        public void AddDBusMenuEntry(SimpleEntry entry)
        {
            if (platformSupportsDBus)
                dBusManager.TrayManager.AddEntry(entry);
        }

        public void RemoveDBusMenuEntry(SimpleEntry entry)
        {
            if (platformSupportsDBus)
                dBusManager.TrayManager.RemoveEntry(entry);
        }

        public void PostSystemNotification(SystemNotification notification)
        {
            if (platformSupportsDBus)
                dBusManager.Notifications.PostAsync(notification);
        }

        private bool platformSupportsDBus => RuntimeInfo.OS == RuntimeInfo.Platform.Linux;

        internal bool AddPlugin(MvisPlugin pl)
        {
            if (avaliablePlugins.Contains(pl) || pl == null) return false;

            if (pl.Version < MinimumPluginVersion)
                Logger.Log($"?????? \"{pl.Name}\" ??????????????????mf-osu?????????, ????????????????????????????????????????????????!", LoggingTarget.Runtime, LogLevel.Important);
            else if (pl.Version > PluginVersion)
                Logger.Log($"?????? \"{pl.Name}\" ?????????????????????mf-osu?????????, ????????????????????????????????????????????????!", LoggingTarget.Runtime, LogLevel.Important);

            avaliablePlugins.Add(pl);
            OnPluginAdd?.Invoke(pl);
            return true;
        }

        internal bool UnLoadPlugin(MvisPlugin pl, bool blockFromFutureLoad = false)
        {
            if (!avaliablePlugins.Contains(pl) || pl == null) return false;

            var provider = providers.Find(p => p.CreatePlugin.GetType() == pl.GetType());

            activePlugins.Remove(pl);
            avaliablePlugins.Remove(pl);
            providers.Remove(provider);

            try
            {
                if (pl is IFunctionBarProvider functionBarProvider)
                    resolver.RemoveFunctionBarProvider(functionBarProvider);

                if (pl is IProvideAudioControlPlugin provideAudioControlPlugin)
                    resolver.RemoveAudioControlProvider(provideAudioControlPlugin);

                pl.UnLoad();
                OnPluginUnLoad?.Invoke(pl);

                var providerAssembly = provider.GetType().Assembly;
                var gameAssembly = GetType().Assembly;

                if (providerAssembly != gameAssembly && blockFromFutureLoad)
                {
                    blockedProviders.Add(providerAssembly.ToString());

                    using (var writer = new StreamWriter(File.OpenWrite(blockedPluginFilePath)))
                    {
                        var serializedString = JsonConvert.SerializeObject(blockedProviders);
                        writer.Write(serializedString);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, $"??????????????????????????????: {e.Message}");

                //??????dispose?????????
                if (pl.Parent is Container container)
                {
                    container.Remove(pl);
                    pl.Dispose();
                }

                //????????????
                resolver.UpdatePluginDictionary(GetAllPlugins(false));
            }

            return true;
        }

        internal bool ActivePlugin(MvisPlugin pl)
        {
            if (!avaliablePlugins.Contains(pl) || activePlugins.Contains(pl) || pl == null) return false;

            if (!activePlugins.Contains(pl))
                activePlugins.Add(pl);

            bool success = pl.Enable();

            if (!success)
                activePlugins.Remove(pl);

            return success;
        }

        internal bool DisablePlugin(MvisPlugin pl)
        {
            if (!avaliablePlugins.Contains(pl) || !activePlugins.Contains(pl) || pl == null) return false;

            activePlugins.Remove(pl);
            bool success = pl.Disable();

            if (!success)
            {
                activePlugins.Add(pl);
                Logger.Log($"????????????\"${pl.Name}\"??????");
            }

            return success;
        }

        public List<MvisPlugin> GetActivePlugins() => activePlugins.ToList();

        /// <summary>
        /// ??????????????????
        /// </summary>
        /// <param name="newInstance">
        /// ??????????????????????????????????????????????????????<br/>
        /// </param>
        /// <returns>?????????????????????????????????</returns>
        public List<MvisPlugin> GetAllPlugins(bool newInstance)
        {
            if (newInstance)
            {
                //bug: ????????????Dispose??????????????????????????????Disposed drawabled may never in the scene graph
                ExpireOldPlugins();

                foreach (var p in providers)
                {
                    avaliablePlugins.Add(p.CreatePlugin);
                }

                resolver.UpdatePluginDictionary(avaliablePlugins.ToList());
            }

            return avaliablePlugins.ToList();
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

        internal List<IFunctionBarProvider> GetAllFunctionBarProviders() => resolver.GetAllFunctionBarProviders();

        internal List<IProvideAudioControlPlugin> GetAllAudioControlPlugin() => resolver.GetAllAudioControlPlugin();

        internal IProvideAudioControlPlugin GetAudioControlByPath([NotNull] string path) => resolver.GetAudioControlPluginByPath(path);
        internal IFunctionBarProvider GetFunctionBarProviderByPath([NotNull] string path) => resolver.GetFunctionBarProviderByPath(path);

        public string ToPath([NotNull] object target) => resolver.ToPath(target);
    }
}
