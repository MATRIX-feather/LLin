using System;
using M.Resources;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Threading;
using osu.Game.Rulesets.Hikariii.Features.Configuration;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins;

namespace osu.Game.Rulesets.Hikariii.Features.ListenerLoader;

public partial class ListenerLoader : AbstractInjector
{
    public static readonly ListenerLoader INSTANCE = new ListenerLoader();

    /// <summary>
    /// 当前注入生效的游戏中 OsuGame 的 HashCode，-1则代表未曾注入过
    /// </summary>
    private static int currentSessionHash = -1;

    public int GetRegisteredSessionHash()
    {
        return currentSessionHash;
    }

    private DependencyContainer? getGameDepManager(OsuGame? gameInstance)
    {
        return gameInstance?.Dependencies as DependencyContainer;
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

                gameInstance.AddRange(new Drawable[]
                {
                    new SentryLoggerDisabler(gameInstance),
                    new GameScreenHandler(),
                    new PreviewTrackHandler()
                });
            }, 1);
        }
        catch (Exception e)
        {
            Logging.LogError(e, "注入游戏时出现错误，一些功能可能不会正常工作！");
            //Logging.Log(e.Message, level: LogLevel.Important);
            return false;
        }

        return true;
    }
}
