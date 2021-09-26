// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LLin.Game.Screens.Mvis.Plugins;
using osu.Framework;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Game;

namespace LLin.Game.Screens
{
    internal class CustomStore : NamespacedResourceStore<byte[]>
    {
        private readonly OsuGameBase gameBase;
        private readonly Storage customStorage;
        private readonly Dictionary<Assembly, Type> loadedAssemblies = new Dictionary<Assembly, Type>();
        private readonly Dictionary<Assembly, Type> loadedMvisPluginAssemblies = new Dictionary<Assembly, Type>();

        public List<MvisPluginProvider> LoadedPluginProviders = new List<MvisPluginProvider>();

        public CustomStore(Storage storage, OsuGameBase gameBase)
            : base(new StorageBackedResourceStore(storage), "custom")
        {
            this.gameBase = gameBase;

            customStorage = storage.GetStorageForDirectory("custom");

            prepareLoad();
        }

        public bool Contains(string path)
        {
            try
            {
                customStorage.Exists(path);
                return true;
            }
            catch (Exception e)
            {
                if (!(e is ArgumentException))
                    Logger.Error(e, "获取文件路径时发生了错误");

                return false;
            }
        }

        private void prepareLoad()
        {
            try
            {
                //加载自带的Mvis插件
                //From RulesetStore
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    string name = assembly.GetName().Name;

                    if (!name.StartsWith("Mvis.Plugin", StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    loadAssembly(assembly);
                }

                //从程序根目录加载
                if (RuntimeInfo.IsDesktop)
                {
                    foreach (var file in Directory.GetFiles(RuntimeInfo.StartupDirectory, "Mvis.Plugin.*.dll"))
                        loadAssembly(Assembly.LoadFrom(file));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, $"载入文件时出现问题: {e.Message}");
            }
        }

        /// <summary>
        /// 加载一个Assembly
        /// </summary>
        /// <param name="assembly">要加载的Assembly</param>
        private void loadAssembly(Assembly assembly)
        {
            if (loadedAssemblies.Any(a => a.Key.FullName == assembly.FullName))
                return;

            var name = Path.GetFileNameWithoutExtension(assembly.Location);
            if (loadedAssemblies.Values.Any(t => Path.GetFileNameWithoutExtension(t.Assembly.Location) == name))
                return;

            try
            {
                foreach (var type in assembly.GetTypes())
                {
                    //Logger.Log($"case: 尝试加载 {type}");

                    if (type.IsSubclassOf(typeof(MvisPluginProvider)))
                    {
                        loadedMvisPluginAssemblies[assembly] = type;
                        loadedAssemblies[assembly] = type;
                        //Logger.Log($"{type}是插件Provider");
                        addMvisPlugin(type, assembly.FullName);
                    }

                    //Logger.Log($"{type}不是任何一个SubClass");
                }

                //添加store
                gameBase.Resources.AddStore(new DllResourceStore(assembly));
            }
            catch (Exception e)
            {
                Logger.Error(e, $"载入 {assembly.FullName} 时出现了问题, 请联系你的插件提供方。");
            }
        }

        /// <summary>
        /// 向CustomStore添加一个插件
        /// </summary>
        /// <param name="pluginType">要添加的插件</param>
        /// <param name="fullName">与pluginType对应的Assembly的fullName</param>
        private void addMvisPlugin(Type pluginType, string fullName)
        {
            //Logger.Log($"载入 {fullName}");

            try
            {
                var providerInstance = (MvisPluginProvider)Activator.CreateInstance(pluginType);
                LoadedPluginProviders.Add(providerInstance);
                //Logger.Log($"[OK] 载入 {fullName}");
            }
            catch (Exception e)
            {
                Logger.Error(e, $"尝试添加插件{fullName}时出现了问题");
            }
        }
    }
}
