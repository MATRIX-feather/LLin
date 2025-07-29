using System;
using M.Resources;
using osu.Framework.Allocation;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Threading;
using osu.Game.Rulesets.Hikariii.Features.Configuration;
using osu.Game.Rulesets.Hikariii.Features.ListenerLoader.Handlers;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Screens.LLin;
using osu.Game.Rulesets.Hikariii.Features.SystemIntegration.DBus;
using osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Media;
using osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Theme;

namespace osu.Game.Rulesets.Hikariii.Features.ListenerLoader;

public partial class ListenerLoader : AbstractHandler
{
    public static readonly ListenerLoader INSTANCE = new ListenerLoader();

    /// <summary>
    /// 当前注入生效的游戏中 OsuGame 的 HashCode，-1则代表未曾注入过
    /// </summary>
    private static int currentSessionHash = -2;

    public int GetRegisteredSessionHash()
    {
        return currentSessionHash;
    }

    private DependencyContainer? getGameDepManager(OsuGame? gameInstance)
    {
        return gameInstance?.Dependencies as DependencyContainer;
    }

    private static AbstractHandler[] injectors()
    {
        return
        [
            new HikariiiFeatureBoxListener(),
            new ScreenHandlerManager(),
            new PreviewTrackHandler()
        ];
    }

    public bool BeginInject(Storage storage, OsuGame? gameInstance, Scheduler scheduler)
    {
        int sessionHashCode = gameInstance?.Toolbar.GetHashCode() ?? -1;

        if (currentSessionHash == sessionHashCode)
        {
            Logging.Log("Duplicate dependency inject call for current session, skipping...");
            return true;
        }

        currentSessionHash = sessionHashCode;

        if (gameInstance?.Dependencies is not DependencyContainer depMgr)
        {
            Logging.Log($"DependencyContainer not found", level: LogLevel.Error);
            return false;
        }

        try
        {
            var plMgr = new LLinPluginManager();

            // Add Resource store
            gameInstance.Resources.AddStore(new DllResourceStore(typeof(HikariiiPlayerRuleset).Assembly));

            try
            {
                //Load MResources
                gameInstance.Resources.AddStore(new DllResourceStore(MResources.ResourceAssembly));
            }
            catch (Exception e)
            {
                Logging.LogError(e, "无法装载M.Resources, 一些意外情况可能发生！");
            }

            var featureManager = new FeatureManager();

            depMgr.CacheAs(typeof(MConfigManager), new MConfigManager(storage));
            depMgr.Cache(plMgr);
            depMgr.Cache(featureManager);

            if (OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
            {
                scheduler.AddDelayed(() =>
                {
                    var dbusIntegration = new DBusIntegration();

                    gameInstance.Add(dbusIntegration);
                    depMgr.Cache(dbusIntegration);
                }, 1);
            }

            scheduler.AddDelayed(() =>
            {
                var mediaIntegration = new MediaIntegration();
                gameInstance.Add(mediaIntegration);
                depMgr.Cache(mediaIntegration);

                var customColors = new CustomColourProvider();
                depMgr.Cache(customColors);

                var systemThemeIntegration = new SystemThemeIntegration();
                gameInstance.Add(systemThemeIntegration);
                depMgr.Cache(systemThemeIntegration);
            }, 1);

            scheduler.AddDelayed(() =>
            {
                try
                {
                    gameInstance.Add(featureManager);
                    gameInstance.Add(plMgr);
                }
                catch (Exception e)
                {
                    Logging.LogError(e, "未能初始化插件管理器, 可能是因为DBus集成没有安装?");
                }

                gameInstance.Add(new SentryLoggerDisabler(gameInstance));

                gameInstance.AddRange(injectors());
            }, 1);
        }
        catch (Exception e)
        {
            Logging.LogError(e, "注入游戏时出现错误，一些功能可能不会正常工作！");
            //Logging.Log(e.Message, level: LogLevel.Important);
            return false;
        }

        Logging.Log("Initial inject done!");

        return true;
    }
}
