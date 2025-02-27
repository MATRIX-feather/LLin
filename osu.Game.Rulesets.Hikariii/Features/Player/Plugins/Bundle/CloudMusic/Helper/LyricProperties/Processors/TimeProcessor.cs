using System;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.CloudMusic.Misc;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.CloudMusic.Helper.LyricProperties.Processors;

public class TimeProcessor : IPropertyProcessor
{
    public bool Process(string propertyInput, Lyric lyricRef)
    {
        if (!char.IsDigit(propertyInput[0]))
            return false;

        lyricRef.Time = toMilliseconds(propertyInput);

        return true;
    }

    private int toMilliseconds(string src)
    {
        string[] spilt = src.Contains(':')
            ? src.Split(':')
            : src.Split('.', 2);

        if (spilt.Length < 2)
        {
            Logging.Log($"无效的时间: \"{src}\"");
            return 0;
        }

        int.TryParse(spilt[0], out int minutes);
        double.TryParse(spilt[1], out double seconds);

        return minutes * 60000 + (int)Math.Round(seconds * 1000);
    }
}
