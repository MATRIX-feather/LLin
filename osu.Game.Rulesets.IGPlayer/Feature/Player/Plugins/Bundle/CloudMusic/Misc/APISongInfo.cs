using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Misc;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.Helper;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.Misc
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class APISongInfo
    {
        public long ID { get; set; }

        public long Duration { get; set; }

        public string? Name { get; set; }

        public List<APIArtistInfo> Artists { get; set; } = [];

        public float TitleSimilarPercentage { get; set; }

        public float ArtistSimilarPercentage { get; set; }

        /// <summary>
        /// 获取网易云歌曲标题和搜索标题的相似度
        /// </summary>
        /// <returns>相似度百分比</returns>
        public void CalculateSimilarPercentage(WorkingBeatmap beatmap)
        {
            string neteaseTitle = Name?.ToLowerInvariant() ?? string.Empty;
            string ourTitle = beatmap.Metadata.Title.ToLowerInvariant() ?? string.Empty;

            float titleSimilarPercentage = LevenshteinDistance.ComputeSimilarPercentage(neteaseTitle, ourTitle);
            ourTitle = beatmap.Metadata.TitleUnicode.ToLowerInvariant() ?? string.Empty;
            float titleSimilarPercentageUnicode = LevenshteinDistance.ComputeSimilarPercentage(neteaseTitle, ourTitle);
            TitleSimilarPercentage = Math.Max(titleSimilarPercentageUnicode, titleSimilarPercentage);

            ourTitle = beatmap.Metadata.GetArtist();
            ArtistSimilarPercentage = Artists.Count(a => ourTitle.Contains(a.Name)) / (float)Artists.Count;
        }

        public string GetArtist() => string.Join(" / ", Artists.Select(a => a.Name));
    }
}
