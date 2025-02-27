using System;
using System.Collections.Generic;
using System.Linq;
using Markdig.Helpers;
using Newtonsoft.Json;

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
            bool propertyDetected = false;
            string propertyName = string.Empty;
            string lyricContent = string.Empty;

            //创建currentLrc
            //可能存在一行歌词多个时间，所以先创建列表
            List<Lyric> processedLyrics = [];

            //处理属性
            foreach (char c in rawLyric)
            {
                if (c == '[')
                {
                    propertyDetected = true;
                    continue;
                }

                //如果检测到']'，那么退出属性检测并处理结果
                if (c == ']' && propertyDetected)
                {
                    propertyDetected = false;

                    //处理属性
                    //时间

                    //如果是时间属性
                    if (propertyName[0].IsDigit())
                    {
                        processedLyrics.Add(new Lyric
                        {
                            Time = propertyName.ToMilliseconds()
                        });
                    }

                    //todo: 在此放置对其他属性的处理逻辑

                    //清空属性名称
                    propertyName = string.Empty;

                    //继续
                    continue;
                }

                //如果是属性，那么添加字符到propertyName，反之则是lyricContent
                if (propertyDetected) propertyName += c;
                else lyricContent += c;

                //Logging.Log($"原始歌词: propertyName: {propertyName} | lyricContent: {lyricContent}");
            }

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

                if (trimmedContent.Contains("纯音乐，请欣赏"))
                {
                    var lyricFirst = outLyrics.First();
                    result.Add(lyricFirst);
                    continue;
                }

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
