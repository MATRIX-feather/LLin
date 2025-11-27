using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Screens.LLin;

public partial class SessionPluginManager : CompositeDrawable
{
    private readonly ConcurrentDictionary<string, LLinPlugin> plugins = new();

    public void LoadPlugins(LLinPluginManager pluginManager)
    {
        pluginManager.GetAllPluginProviders().ForEach(kvp => plugins[kvp.Key] = kvp.Value.CreatePlugin());
    }

    public X? GetPluginWithType<X>(string id)
        where X : class
    {
        var plugin = plugins!.GetValueOrDefault(id, null);
        return plugin as X;
    }

    public X GetPluginWithTypeOrThrow<X>(string id)
        where X : class
    {
        var result = GetPluginWithType<X>(id);

        return result ?? throw new Exception($"Could not find plugin with id {id}");
    }

    public Dictionary<string, LLinPlugin> PluginsDictionary()
    {
        return new Dictionary<string, LLinPlugin>(plugins);
    }

    public void EnablePlugin(LLinPlugin plugin)
    {
        plugin.Enable();
    }

    public void DisablePlugin(LLinPlugin plugin)
    {
        plugin.Disable();
    }

    public Action<LLinPlugin> OnPluginAdded;
    public Action<LLinPlugin> OnPluginRemoved;
}
