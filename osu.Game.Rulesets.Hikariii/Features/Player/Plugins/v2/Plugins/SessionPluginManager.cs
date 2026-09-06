using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Extensions;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.BuiltIn.Core;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins;

public partial class SessionPluginManager : CompositeDrawable
{
    private readonly ConcurrentDictionary<string, DrawableHikariiiPlugin> trackingPlugins = new();

    private readonly Bindable<string> configEnabledPluginNames = new();
    private readonly List<string> allowedPlugins = [];

    [Resolved]
    private IHikariiiPluginManager plugins { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load(HikariiiCoreConfigManager config)
    {
        config.BindWith(HikariiiCoreSetting.EnabledPlugins, configEnabledPluginNames);
        configEnabledPluginNames.BindValueChanged(this.onConfigChanged);
    }

    private void onConfigChanged(ValueChangedEvent<string> e)
    {
        string[] fullList = configEnabledPluginNames.Value.Split(" ");

        allowedPlugins.Clear();
        allowedPlugins.AddRange(fullList);

        string[] newlyEnabledPlugins = fullList.Concat(allowedPlugins)
                                               .Distinct()
                                               .ToArray();

        string[] newlyDisabledPlugins = fullList.Except(allowedPlugins)
                                                .ToArray();

        newlyEnabledPlugins.ForEach(n => EnablePlugin(n));
        newlyDisabledPlugins.ForEach(DisablePlugin);
    }

    public IHikariiiPluginProvider[] AllowedPluginProviders()
    {
        return
        [
            .. allowedPlugins.Select(name => plugins.GetPluginProvider(name))
                             .Where(r => r != null)
                             .Select(r => r!)
        ];
    }

    public DrawableHikariiiPlugin? GetPlugin(string id)
    {
        return trackingPlugins!.GetValueOrDefault(id, null);
    }

    public DrawableHikariiiPlugin GetPluginOrThrow(string id)
    {
        return GetPlugin(id) ?? throw new Exception($"Could not find plugin with id {id}");
    }

    public X? GetPluginWithType<X>(string id)
        where X : class
    {
        return GetPlugin(id) as X;
    }

    public X GetPluginWithTypeOrThrow<X>(string id)
        where X : class
    {
        var result = GetPluginWithType<X>(id);

        return result ?? throw new Exception($"Could not find plugin with id {id}");
    }

    public Dictionary<string, DrawableHikariiiPlugin> PluginsDictionary()
    {
        return new Dictionary<string, DrawableHikariiiPlugin>(trackingPlugins);
    }

    public DrawableHikariiiPlugin EnablePlugin(string id)
    {
        DrawableHikariiiPlugin? existing = null;
        if (trackingPlugins.TryGetValue(id, out existing))
            return existing;

        var plugin = plugins.GetPluginProviderOrThrow(id).CreateDrawablePlugin();
        trackingPlugins[id] = plugin;
        OnPluginEnable?.Invoke((id, plugin));
        return plugin;
    }

    public void DisablePlugin(string id)
    {
        DrawableHikariiiPlugin? plugin;
        trackingPlugins.Remove(id, out plugin);

        if (plugin == null) return;

        OnPluginDisable?.Invoke((id, plugin));
    }

    public bool IsPluginEnabled(string id)
    {
        return trackingPlugins.ContainsKey(id);
    }

    public Action<(string id, DrawableHikariiiPlugin drawablePlugin)> OnPluginEnable;
    public Action<(string id, DrawableHikariiiPlugin drawablePlugin)> OnPluginDisable;
}
