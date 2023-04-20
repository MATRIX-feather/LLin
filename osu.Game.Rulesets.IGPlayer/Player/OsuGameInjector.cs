using System;
using M.Resources;
using osu.Framework.Allocation;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Threading;
using osu.Game.Rulesets.IGPlayer.Configuration;
using osu.Game.Rulesets.IGPlayer.Player.Plugins;

namespace osu.Game.Rulesets.IGPlayer.Player;

public class OsuGameInjector
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
            var ress = new MResources();

            depMgr.CacheAs(typeof(MConfigManager), new MConfigManager(storage));
            depMgr.Cache(plMgr);

            scheduler.AddDelayed(() => gameInstance.Add(plMgr), 1);
        }
        catch (Exception e)
        {
            Logger.Log(e.Message, level: LogLevel.Important);
            return false;
        }

        return true;
    }
}
