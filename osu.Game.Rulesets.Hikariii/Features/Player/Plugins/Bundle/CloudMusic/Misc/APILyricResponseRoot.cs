using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using osu.Framework.Development;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.CloudMusic.Helper.LyricProperties;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.CloudMusic.Misc
{
    public class APILyricResponseRoot : IDisposable
    {
        [JsonProperty("lrc")]
        public LyricInfo? LyricInfo;

        /// <summary>
        /// 网易云中歌词和翻译歌词是分开的，存储和下载歌词是用TLyricInfo获取翻译
        /// </summary>
        [JsonProperty("tlyric")]
        public LyricInfo? TLyricInfo;

        [JsonProperty("localOffset")]
        public double LocalOffset;

        [JsonIgnore]
        private List<string>? lyrics => LyricInfo?.RawLyric?.Split("\n", StringSplitOptions.RemoveEmptyEntries).ToList();

        [JsonIgnore]
        private List<string>? translateLyrics => TLyricInfo?.RawLyric?.Split("\n", StringSplitOptions.RemoveEmptyEntries).ToList();

        /// <summary>
        ///
        /// </summary>
        /// <param name="rawLyric"></param>
        /// <returns>一个装有<see cref="Lyric"/>列表，以及他们所对应的字符串内容</returns>
        private (List<Lyric>, string) processRaw(string rawLyric)
        {
            string lyricContent;

            //创建currentLrc
            //可能存在一行歌词多个时间，所以先创建列表
            List<Lyric> processedLyrics = [];

            int nextIndex = 0;

            while (true)
            {
                int nextOpenQuote = rawLyric.IndexOf('[', nextIndex);

                if (nextOpenQuote == -1)
                {
                    lyricContent = rawLyric[nextIndex..];
                    break;
                }

                int nextCloseQuote = rawLyric.IndexOf(']', nextOpenQuote + 1);

                if (nextCloseQuote == -1)
                {
                    if (DebugUtils.IsDebugBuild)
                        Logging.Log($"DEBUG 找到了 ’[’, 但是没有下一个 ’]’... 这对吗？正在返回剩下的字符串 --> '{rawLyric}'");

                    lyricContent = rawLyric[nextOpenQuote..];
                    break;
                }

                nextIndex = nextCloseQuote + 1;

                // 截取property

                int length = nextCloseQuote - nextOpenQuote - 1;
                bool exceedRawLyricLimit = nextOpenQuote + 1 + length >= rawLyric.Length;

                string property = exceedRawLyricLimit
                    ? string.Empty
                    : rawLyric.Substring(nextOpenQuote + 1, length);

                // 属性是空的，BadLyric!
                if (property == string.Empty)
                {
                    if (DebugUtils.IsDebugBuild)
                        Logging.Log($"DEBUG Bad code! We reached the limit! '{rawLyric}'");

                    lyricContent = rawLyric[nextOpenQuote..];
                    break;
                }

                // 处理property
                Lyric initialLyric = new Lyric();

                if (!PropertyProcessorManager.INSTANCE.Process(property, initialLyric))
                {
                    Logging.Log($"Failed to process lyric propety '{property}', none matched!");
                    continue;
                }

                processedLyrics.Add(initialLyric);
            }

            if (lyricContent == "纯音乐，请欣赏")
                lyricContent = string.Empty;

            return (processedLyrics, lyricContent);
        }

        public List<Lyric> ToLyricList()
        {
            var result = new List<Lyric>();

            if (lyrics == null) return result;

            //蠢办法，但起码比之前有用(
            //先处理原始歌词信息
            foreach (string lyricString in lyrics)
            {
                (var outLyrics, string? trimmedContent) = processRaw(lyricString);

                if (outLyrics.Count == 0)
                    continue;

                //最后，设置歌词内容并添加到result
                foreach (var lyric in outLyrics)
                {
                    lyric.Content = trimmedContent;

                    //Logging.Log($"添加歌词: {lyric}");

                    result.Add(lyric);
                }
            }

            //再处理翻译歌词
            //有必要进行分开处理，因为在返回的数据里歌词和翻译不总是一一对应
            if (translateLyrics != null)
            {
                foreach (string tlyricString in translateLyrics)
                {
                    //Logging.Log($"处理翻译歌词: {tlyricString}");

                    (var outLyrics, string? trimmedContent) = processRaw(tlyricString);

                    foreach (Lyric lrc in outLyrics.SelectMany(lyric => result.FindAll(l => Math.Abs(l.Time - lyric.Time) < 0.01d)))
                        lrc.TranslatedString = trimmedContent;
                }
            }

            result.Sort((l1, l2) => l2.CompareTo(l1));

            return result;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
