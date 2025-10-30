using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SettingsItems;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins
{
    //todo: 如果可以的话，我希望 LLinPluginProvider 能有个泛型，像是 LLinPluginProvider<C> where C : IPluginConfigManager 这样
    //      但是这样的话我不知道如何在 LLinPluginManager 那里将所有 provider 都注册为 IPluginConfigManager
    //      😢
    public abstract class LLinPluginProvider
    {
        /// <summary>
        /// 要提供的插件
        /// </summary>
        public abstract LLinPlugin CreatePlugin();

        /// <summary>
        /// Description for this plugin
        /// </summary>
        public abstract PluginDescription GetDescription();

        /// <summary>
        /// 此插件的 ID
        /// </summary>
        public abstract string Identifier();

        /// <summary>
        /// Create config manager for this plugin
        /// </summary>
        public abstract IPluginConfigManager CreateConfigManager(Storage storage);

        /// <summary>
        /// Get settings entries for this plugin
        /// </summary>
        /// <param name="config">Plugin's config manager</param>
        public virtual SettingsEntry[] GetSettingEntries(IPluginConfigManager config)
        {
            return [];
        }

        /// <summary>
        /// If this plugin provider having any persistent settings entry, you can initialize them here
        /// </summary>
        /// <param name="config">Plugin's config manager</param>
        public virtual void EarlyInitSettingEntries(IPluginConfigManager config)
        {
        }
/*
        /// <summary>
        /// workaround: List entries
        /// </summary>
        public override string ToString()
        {
            var desc = GetDescription();

            return $"{desc.Name} ({desc.AuthorString()})";
        }*/
    }
}
