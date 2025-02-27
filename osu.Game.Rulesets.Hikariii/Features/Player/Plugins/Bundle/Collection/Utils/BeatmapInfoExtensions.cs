using System;
using osu.Game.Beatmaps;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection.Utils
{
    public static class BeatmapInfoExtensions
    {
        //只能先这样做了，BeatmapManager的GetWorkingBeatmap要求参数是BeatmapInfo，但是
        //BeatmapCollection.Beatmaps从BeatmapInfo变成了IBeatmapInfo
        public static BeatmapInfo AsBeatmapInfo(this IBeatmapInfo iInfo)
        {
            if (iInfo is BeatmapInfo beatmapInfo) return beatmapInfo;

            throw new InvalidCastException($"{iInfo} 不是 BeatmapInfo");
        }
    }
}
