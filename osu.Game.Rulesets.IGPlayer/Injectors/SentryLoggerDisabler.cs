using System;
using osu.Framework.Allocation;
using osu.Framework.Logging;
using osu.Game.Utils;

namespace osu.Game.Rulesets.IGPlayer.Injectors;

public partial class SentryLoggerDisabler : AbstractInjector
{
    [BackgroundDependencyLoader(permitNulls: true)]
    private void load(OsuGame game)
    {
        try
        {
            disableSentryLogger(game);
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
        Logger.Log("成功禁用SentryLogger!");
    }
}
