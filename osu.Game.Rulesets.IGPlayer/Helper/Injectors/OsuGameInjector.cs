using System;
using M.Resources;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Threading;
using osu.Game.Rulesets.IGPlayer.Feature;
using osu.Game.Rulesets.IGPlayer.Feature.Gosumemory;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins;
using osu.Game.Rulesets.IGPlayer.Helper.Configuration;

namespace osu.Game.Rulesets.IGPlayer.Helper.Injectors;

public partial class OsuGameInjector : AbstractInjector
{
    /// <summary>
    /// 当前注入生效的游戏中 OsuGame 的 HashCode，-1则代表未曾注入过
    /// </summary>
    private static int currentSessionHash = -1;

    public static int GetRegisteredSessionHash()
    {
        return currentSessionHash;
    }

    public static DependencyContainer? GetGameDepManager(OsuGame? gameInstance)
    {
        return gameInstance?.Dependencies as DependencyContainer;
    }

    public static bool InjectDependencies(Storage storage, OsuGame gameInstance, Scheduler scheduler)
    {
        int sessionHashCode = gameInstance.Toolbar.GetHashCode();

        if (currentSessionHash == sessionHashCode)
        {
            Logging.Log($"Duplicate dependency inject call for current session, skipping...");
            return true;
        }

        currentSessionHash = sessionHashCode;

        var depMgr = GetGameDepManager(gameInstance);

        if (depMgr == null)
        {
            Logging.Log($"DependencyContainer not found", level: LogLevel.Error);
            return false;
        }

        try
        {
            Logging.Log("Add rs store");
            // Add Resource store
            gameInstance.Resources.AddStore(new DllResourceStore(typeof(IGPlayerRuleset).Assembly));

            Logging.Log("Try MResources");
            try
            {
                //Load MResources
                gameInstance.Resources.AddStore(new DllResourceStore(MResources.ResourceAssembly));
            }
            catch (Exception e)
            {
                Logging.LogError(e, "无法装载M.Resources, 一些意外情况可能发生！");
            }

            Logging.Log("Feature Manager");
            var featureManager = new FeatureManager();

            Logging.Log("Cache MConfig");
            depMgr.CacheAs(typeof(MConfigManager), new MConfigManager(storage));

            Logging.Log("Cache feature");
            depMgr.Cache(featureManager);

            Logging.Log("Add delayed 1");
            scheduler.AddDelayed(() =>
            {
                try
                {
                    Logging.Log("Add instance to game");
                    gameInstance.Add(featureManager);
                }
                catch (Exception e)
                {
                    Logging.LogError(e, "未能初始化功能管理器！");
                }

                Logging.Log("Add range");
                gameInstance.AddRange(new Drawable[]
                {
                    new SentryLoggerDisabler(gameInstance),
                    new GameScreenInjector(),
                    new PreviewTrackInjector()
                });

                if (featureManager.CanUseGlazerMemory.Value)
                    gameInstance.Add(new GosuCompatInjector());
            }, 1);

            Logging.Log("Add delayed 2");
            scheduler.AddDelayed(() =>
            {
                LLinPluginManager? plMgr = null;

                try
                {
                    plMgr = new LLinPluginManager();
                    depMgr.Cache(plMgr);
                    gameInstance.Add(plMgr);
                }
                catch (Exception e)
                {
                    plMgr = null;
                    Logging.LogError(e, "未能初始化插件管理器, 可能是因为DBus集成没有安装?");
                }

            }, 1);
        }
        catch (Exception e)
        {
            Logging.LogError(e, "注入游戏时出现错误，一些功能可能不会正常工作！");
            Logging.Log(e.Message, level: LogLevel.Important);
            Logging.Log(e.StackTrace ?? "<Null Stacktrace>");

            return false;
        }

        return true;
    }
}
