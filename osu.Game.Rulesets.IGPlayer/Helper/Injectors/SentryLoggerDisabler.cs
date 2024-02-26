using System;
using osu.Framework.Allocation;
using osu.Game.Utils;

namespace osu.Game.Rulesets.IGPlayer.Helper.Injectors;

public partial class SentryLoggerDisabler : AbstractInjector
{
    public SentryLoggerDisabler(OsuGame gameInstance)
    {
        try
        {
            disableSentryLogger(gameInstance);
        }
        catch (Exception e)
        {
            Logging.LogError(e, "未能禁用 SentryLogger :(");
        }
    }

    private void disableSentryLogger(OsuGame game)
    {
        var field = this.FindFieldInstance(game, typeof(SentryLogger));
        if (field == null) throw new NullDependencyException("没有找到SentryLogger");

        object? val = field.GetValue(game);
        if (val is not SentryLogger sentryLogger) throw new NullDependencyException($"获取的对象不是SentryLogger: {val}");

        sentryLogger.Dispose();
        Logging.Log("成功禁用SentryLogger!");
    }
}
