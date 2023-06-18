using System;
using M.Resources;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Threading;
using osu.Game.Rulesets.IGPlayer.Configuration;
using osu.Game.Rulesets.IGPlayer.Feature;
using osu.Game.Rulesets.IGPlayer.Feature.Gosumemory;
using osu.Game.Rulesets.IGPlayer.Player.Plugins;

namespace osu.Game.Rulesets.IGPlayer.Injectors;

public partial class OsuGameInjector : AbstractInjector
{
    public static DependencyContainer? GetGameDepManager(OsuGame? gameInstance)
    {
        return gameInstance?.Dependencies as DependencyContainer;
    }

    public static bool InjectDependencies(Storage storage, OsuGame gameInstance, Scheduler scheduler)
    {
        var depMgr = GetGameDepManager(gameInstance);

        if (depMgr == null)
        {
            Logger.Log("[IGPlayer] DependencyContainer not found");
            return false;
        }

        if (depMgr.Get<MConfigManager>() != null) return true;

        try
        {
            var plMgr = new LLinPluginManager();

            // Add Resource store
            gameInstance.Resources.AddStore(new DllResourceStore(typeof(IGPlayerRuleset).Assembly));

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
                    new SentryLoggerDisabler(),
                    new GameScreenInjector(),
                    new PreviewTrackInjector()
                });

                if (featureManager.CanUseGlazerMemory.Value)
                    gameInstance.Add(new GosuCompatInjector());
            }, 1);
        }
        catch (Exception e)
        {
            Logger.Log(e.Message, level: LogLevel.Important);
            return false;
        }

        return true;
    }
}
