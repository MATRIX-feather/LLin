using System;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SettingsItems;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.BuiltIn.Core;

public class HikariiiCore : IHikariiiPluginProvider
{
    public const string ID = "hikariii-core";

    public string GetID() => ID;

    public IPluginConfigManager CreatePluginConfig(Storage storageAccess)
    {
        return new HikariiiCoreConfigManager(storageAccess, this);
    }

    public Type GetPluginConfigType()
    {
        return typeof(HikariiiCoreConfigManager);
    }

    public PluginDescription GetPluginDescription() => new("Hikariii", "播放器基础设置", ["mfosu"]);

    public DrawableHikariiiPlugin CreateDrawablePlugin()
    {
        return new DrawableHikariiiCore();
    }

    private volatile SettingsEntry[]? settingsEntries;

    public readonly IBindable<IHikariiiPluginProvider> AudioController = new Bindable<IHikariiiPluginProvider>();
    public readonly IBindable<IHikariiiPluginProvider> FunctionBarProvider = new Bindable<IHikariiiPluginProvider>();

    private SettingsEntry[] createSettingsEntriesIfNotSet(HikariiiCoreConfigManager config)
    {
        if (settingsEntries != null)
            return settingsEntries;

        ListSettingsEntry<IHikariiiPluginProvider> funcBarEntry;
        ListSettingsEntry<IHikariiiPluginProvider> audioControllerEntry;

        SettingsEntry[] entries =
        [
            new NumberSettingsEntry<float>
            {
                Name = "背景模糊",
                Bindable = config.GetBindable<float>(HikariiiCoreSetting.BackgroundBlur),
                DisplayAsPercentage = true,
                KeyboardStep = 0.01f,
            },
            new NumberSettingsEntry<float>
            {
                Name = "空闲时的背景亮度",
                Bindable = config.GetBindable<float>(HikariiiCoreSetting.IdleBackgroundDim),
                DisplayAsPercentage = true,
                KeyboardStep = 0.01f,
            },
            new BooleanSettingsEntry
            {
                Name = "启用背景动画",
                Bindable = config.GetBindable<bool>(HikariiiCoreSetting.TrianglesInBackground),
                Description = "如果条件允许,播放器将会在背景显示动画"
            },
            funcBarEntry = new ListSettingsEntry<IHikariiiPluginProvider>
            {
                Name = "底栏插件",
                Bindable = (IBindable)FunctionBarProvider
            },
            new BooleanSettingsEntry
            {
                Name = "节能模式",
                Bindable = config.GetBindable<bool>(HikariiiCoreSetting.EcoMode),
                Description = "启用后，将在进入播放器时自动启用垂直同步和单线程，并在退出时恢复进入前的状态",
                Icon = FontAwesome.Solid.Leaf
            },
            new BooleanSettingsEntry
            {
                Name = "使用空心三角",
                Bindable = config.GetBindable<bool>(HikariiiCoreSetting.HollowTriangles)
            },
            new BooleanSettingsEntry
            {
                Name = "启用进、退场动画",
                Description = "嗯...至少有人说挺炫酷的？",
                Bindable = config.GetBindable<bool>(HikariiiCoreSetting.EnableFancyIntroOutro)
            },
            new NumberSettingsEntry<float>
            {
                Name = "播放器设置最大宽度",
                Bindable = config.GetBindable<float>(HikariiiCoreSetting.SettingsMaxWidth),
                DisplayAsPercentage = true,
                KeyboardStep = 0.01f,
                CommitOnMouseRelease = true
            },
            audioControllerEntry = new ListSettingsEntry<IHikariiiPluginProvider>
            {
                Name = "音乐控制插件",
                Bindable = (IBindable)AudioController
            },
            new NumberSettingsEntry<double>
            {
                Name = "播放速度",
                Bindable = config.GetBindable<double>(HikariiiCoreSetting.PlaybackSpeed),
                KeyboardStep = 0.01f,
                DisplayAsPercentage = true,
                //TransferValueOnCommit = true
            },
            new BooleanSettingsEntry
            {
                Name = "调整音调",
                Bindable = config.GetBindable<bool>(HikariiiCoreSetting.AdjustTrackPitch),
                Description = "暂不支持调整故事版的音调"
            },
            new BooleanSettingsEntry
            {
                Name = "夜核节拍器",
                Bindable = config.GetBindable<bool>(HikariiiCoreSetting.NightcoreBeat),
                Description = "动次打次动次打次"
            }
        ];

        settingsEntries = entries;
        return entries;
    }

    public SettingsEntry[] GetSettingsEntries(IPluginConfigManager config)
    {
        return createSettingsEntriesIfNotSet((HikariiiCoreConfigManager)config);
    }
}
