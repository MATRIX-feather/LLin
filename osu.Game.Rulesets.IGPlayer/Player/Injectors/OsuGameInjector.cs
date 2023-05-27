using System;
using M.Resources;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Threading;
using osu.Game.Rulesets.IGPlayer.Configuration;
using osu.Game.Rulesets.IGPlayer.Player.Injectors;
using osu.Game.Rulesets.IGPlayer.Player.Plugins;

namespace osu.Game.Rulesets.IGPlayer.Player;

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

            //Load MResources
            try
            {
                var resources = new MResources();
            }
            catch (Exception e)
            {
                Logging.LogError(e, "无法装载M.Resources, 一些插件功能可能不会生效");
            }

            depMgr.CacheAs(typeof(MConfigManager), new MConfigManager(storage));
            depMgr.Cache(plMgr);

            scheduler.AddDelayed(() =>
            {
                try
                {
                    gameInstance.Add(plMgr);
                }
                catch (Exception e)
                {
                    Logging.LogError(e, "未能初始化插件管理器, 可能是因为DBus集成没有安装?");
                }
            }, 1);

            scheduler.AddDelayed(() => gameInstance.AddRange(new Drawable[]
            {
                new SentryLoggerDisabler(),
                new GameScreenInjector(),
                new PreviewTrackInjector()
            }), 1);
        }
        catch (Exception e)
        {
            Logger.Log(e.Message, level: LogLevel.Important);
            return false;
        }

        return true;
    }
}
