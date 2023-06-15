using Newtonsoft.Json;
using osu.Framework.Graphics.Audio;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Extensions;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Data.Menu
{
    public struct GosuBeatmapInfo : ISelfUpdatableFromBeatmap, ISelfUpdatableFromAudio
    {
        [JsonProperty("time")]
        public GosuTimeInfo Time;

        [JsonProperty("id")]
        public int BeatmapOnlineId;

        [JsonProperty("set")]
        public int BeatmapSetOnlineId;

        [JsonProperty("md5")]
        public string MD5;

        [JsonProperty("metadata")]
        public GosuBeatmapMetadata MetaData;

        [JsonProperty("stats")]
        public GosuBeatmapStats Stats;

        [JsonProperty("path")]
        public GosuPath Path;

        [JsonProperty("rankedStatus")]
        public short RankedStatus;

        public void UpdateTrack(DrawableTrack track)
        {
            this.Time.TrackLength = (int)track.Length;
            this.Time.TrackProgress = (int)track.CurrentTime;
        }

        public void UpdateBeatmap(WorkingBeatmap beatmap)
        {
            this.BeatmapOnlineId = beatmap.BeatmapInfo.OnlineID;
            this.MD5 = beatmap.BeatmapInfo.MD5Hash;

            //this.Time.firstObject = beatmap.BeatmapInfo.CountdownOffset;
            this.Time.BeatmapLength = (int)beatmap.BeatmapInfo.Length;

            var metadata = beatmap.Metadata;
            this.MetaData.Artist = metadata.GetArtist();
            this.MetaData.ArtistRomainsed = metadata.Artist;
            this.MetaData.Title = metadata.GetTitle();
            this.MetaData.TitleRomainsed = metadata.Title;
            this.MetaData.Mapper = metadata.Author.Username;
            this.MetaData.DiffName = beatmap.BeatmapInfo.DifficultyName;

            var diffInf = beatmap.BeatmapInfo.Difficulty;
            this.Stats.AR = diffInf.ApproachRate;
            this.Stats.CS = diffInf.CircleSize;
            this.Stats.HP = diffInf.DrainRate;
            this.Stats.OD = diffInf.OverallDifficulty;
            this.Stats.SR = (float)beatmap.BeatmapInfo.StarRating;

            short rankingStatus;

            switch (beatmap.BeatmapInfo.Status)
            {
                default:
                case BeatmapOnlineStatus.LocallyModified:
                case BeatmapOnlineStatus.None:
                    rankingStatus = 1;
                    break;

                case BeatmapOnlineStatus.Pending:
                case BeatmapOnlineStatus.WIP:
                case BeatmapOnlineStatus.Graveyard:
                    rankingStatus = 2;
                    break;

                case BeatmapOnlineStatus.Ranked:
                    rankingStatus = 4;
                    break;

                case BeatmapOnlineStatus.Qualified:
                case BeatmapOnlineStatus.Approved:
                    rankingStatus = 5;
                    break;

                case BeatmapOnlineStatus.Loved:
                    rankingStatus = 7;
                    break;
            }

            this.RankedStatus = rankingStatus;
        }
    }

    public struct GosuPath
    {
        [JsonProperty("full")]
        public string BackgroundPath;

        [JsonProperty("folder")]
        public string BeatmapFolder;

        [JsonProperty("file")]
        public string BeatmapFile;

        [JsonProperty("bg")]
        public string BgPath;

        [JsonProperty("audio")]
        public string AudioPath;
    }

    public struct GosuTimeInfo
    {
        [JsonProperty("firstObj")]
        public int FirstObject;

        [JsonProperty("current")]
        public int TrackProgress;

        [JsonProperty("full")]
        public int BeatmapLength;

        [JsonProperty("mp3")]
        public int TrackLength;
    }

    public struct GosuBeatmapMetadata
    {
        [JsonProperty("artist")]
        public string Artist;

        [JsonProperty("artistOriginal")]
        public string ArtistRomainsed;

        [JsonProperty("title")]
        public string Title;

        [JsonProperty("titleOriginal")]
        public string TitleRomainsed;

        [JsonProperty("mapper")]
        public string Mapper;

        [JsonProperty("difficulty")]
        public string DiffName;
    }

    public struct GosuBPMInfo
    {
        [JsonProperty("min")]
        public int Min;

        [JsonProperty("max")]
        public int Max;
    }

    public struct GosuBeatmapStats
    {
        [JsonProperty]
        public float AR;

        [JsonProperty]
        public float CS;

        [JsonProperty]
        public float OD;

        [JsonProperty]
        public float HP;

        /// <summary>
        /// Star Rating
        /// </summary>
        [JsonProperty]
        public float SR;

        [JsonProperty]
        public GosuBPMInfo BPM;

        [JsonProperty("maxCombo")]
        public int MaxCombo;

        [JsonProperty("fullSR")]
        public float FullSR => SR;

        /// <summary>
        /// MemoryAR 是当前状态下的 AR，以此类推?
        /// </summary>

        [JsonProperty("memoryAR")]
        public float memAR => AR;

        [JsonProperty("memoryCS")]
        public float memCS => CS;

        [JsonProperty("memoryOD")]
        public float memOD => OD;

        [JsonProperty("memoryHP")]
        public float memHP => HP;
    }
}
