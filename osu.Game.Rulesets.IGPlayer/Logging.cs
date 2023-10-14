using System;
using osu.Framework.Logging;

namespace osu.Game.Rulesets.IGPlayer;

public class Logging
{
    public static void LogError(Exception e, string? message = null)
    {
        while (true)
        {
            Logger.Log($"[{Constants.LOG_PREFIX}] {(string.IsNullOrEmpty(message) ? "" : $"{message}: ")}{e.Message}", level: LogLevel.Important);
            Logger.Log(e.StackTrace);

            if (e.InnerException != null)
            {
                e = e.InnerException;
                message = null;
                continue;
            }

            break;
        }
    }
}
