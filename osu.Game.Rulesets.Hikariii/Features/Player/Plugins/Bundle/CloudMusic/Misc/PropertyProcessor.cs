using System;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.CloudMusic.Misc
{
    public static class PropertyProcessor
    {
        public static int ToMilliseconds(string src)
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
}
