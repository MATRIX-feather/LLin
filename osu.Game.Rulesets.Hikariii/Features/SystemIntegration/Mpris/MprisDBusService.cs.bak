#nullable disable

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using M.DBus;
using osu.Framework.Bindables;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hikariii.Features.Player.Misc;
using osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Mpris.Interfaces;
using osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Mpris.Properties;
using Tmds.DBus;

namespace osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Mpris
{
    public class MprisPlayerService : IMDBusObject, IPlayerController, IMediaPlayer2
    {
        public ObjectPath ObjectPath => PATH;
        public static readonly ObjectPath PATH = new ObjectPath("/org/mpris/MediaPlayer2");

        public string CustomRegisterName => "org.mpris.MediaPlayer2.mfosu";
        public bool IsService => true;

        private readonly PlayerProperties playerProperties = new PlayerProperties();

        private readonly MediaPlayer2Properties mp2Properties = new MediaPlayer2Properties();

        internal Storage Storage { get; set; }

        #region 用于导出到游戏的控制

        public Action Next;
        public Action Previous;
        public Action Pause;
        public Action Stop;
        public Action Play;

        public Action<bool> LoopChange;

        public Action<long> Seek;
        public Action<long> SetPosition;
        public Action<string> OpenUri;

        public Action WindowRaise;
        public Action Quit;

        public Action<double> OnVolumeSet;
        public Action<bool> OnRandom;

        /// <summary>
        /// 是否允许通过DBus更改属性
        /// </summary>
        public bool AllowSet = false;

        #endregion

        private bool trackRunning;

        /// <summary>
        /// 暂停、播放显示
        /// </summary>
        internal bool TrackRunning
        {
            set
            {
                if (trackRunning == value) return;

                trackRunning = value;

                playerProperties._PlaybackStatus = value ? "Playing" : "Paused";
                OnPropertiesChanged?.Invoke(PropertyChanges.ForProperty("PlaybackStatus", playerProperties.PlaybackStatus));
            }
        }

        public bool TrackLooping
        {
            set
            {
                playerProperties._LoopStatus = value ? MprisStatusStrings.LOOP_STATUS_SINGLE : MprisStatusStrings.LOOP_STATUS_NONE;
                OnPropertiesChanged?.Invoke(PropertyChanges.ForProperty("LoopStatus", playerProperties._LoopStatus));
            }
        }

        internal bool AudioControlDisabled { get; set; }

        private bool beatmapDisabled;

        internal bool BeatmapDisabled
        {
            get => beatmapDisabled;
            set
            {
                beatmapDisabled = value;

                playerProperties.CanGoNext = !value;
                playerProperties.CanGoPrevious = !value;
                playerProperties.CanSeek = !value;

                OnPropertiesChanged?.Invoke(PropertyChanges.ForProperty("CanGoNext", playerProperties.CanGoNext));
                OnPropertiesChanged?.Invoke(PropertyChanges.ForProperty("CanGoPrevious", playerProperties.CanGoPrevious));
                OnPropertiesChanged?.Invoke(PropertyChanges.ForProperty("CanSeek", playerProperties.CanSeek));
            }
        }

        internal bool Shuffle
        {
            set
            {
                playerProperties.Shuffle = value;
                OnPropertiesChanged?.Invoke(PropertyChanges.ForProperty("Shuffle", playerProperties.Shuffle));
            }
        }

        internal long Progress
        {
            set
            {
                playerProperties.Position = value;
                OnPropertiesChanged?.Invoke(PropertyChanges.ForProperty("Position", playerProperties.Position));
            }
        }

        internal void TriggerAll()
        {
            var members = ServiceUtils.GetMembers(playerProperties);

            foreach (var keyValuePair in members)
            {
                object val = ServiceUtils.GetValueFor(playerProperties, keyValuePair.Key, members);

                Logger.Log($"Triggering {keyValuePair.Key} --> {val}");

                if (val != null)
                    OnPropertiesChanged?.Invoke(PropertyChanges.ForProperty(keyValuePair.Key, val));
            }

            var m2Members = ServiceUtils.GetMembers(mp2Properties);

            foreach (var keyValuePair in m2Members)
            {
                object val = ServiceUtils.GetValueFor(mp2Properties, keyValuePair.Key, m2Members);

                Logger.Log($"Triggering {keyValuePair.Key} --> {val}");

                if (val != null)
                    OnPropertiesChanged?.Invoke(PropertyChanges.ForProperty(keyValuePair.Key, val));
            }
        }

        //Position可以直接调用OnPropertiesChanged
        //但Length是在Metadata中的，无法直接调用
        //因此手动调用对Metadata的OnPropertiesChanged
        internal long TrackLength
        {
            set
            {
                long oldval = (long)playerProperties.Metadata["mpris:length"];

                if (oldval == value) return;

                playerProperties.Metadata["mpris:length"] = value;
                OnPropertiesChanged?.Invoke(PropertyChanges.ForProperty(nameof(playerProperties.Metadata), playerProperties.Metadata));
            }
        }

        /// <summary>
        /// 元数据
        /// </summary>
        private WorkingBeatmap beatmap;

        internal WorkingBeatmap Beatmap
        {
            get => beatmap;
            set
            {
                beatmap = value;

                var info = value.BeatmapInfo;
                playerProperties.Metadata["xesam:artist"] = new[] { info.Metadata.GetArtist() };
                playerProperties.Metadata["xesam:title"] = info.Metadata.GetTitle().Title;
                playerProperties.Metadata["xesam:album"] = info.DifficultyName;
                playerProperties.Metadata["xesam:audioBPM"] = info.BPM;

                playerProperties.Metadata["mpris:artUrl"] = resolveBeatmapCoverUrl(value);
                playerProperties.Metadata["mpris:trackid"] = new ObjectPath("/not/implemented/yet");

                OnPropertiesChanged?.Invoke(PropertyChanges.ForProperty(nameof(playerProperties.Metadata), playerProperties.Metadata));
            }
        }

        [CanBeNull]
        private Bindable<bool> useAvatarLogoAsDefault;

        internal Bindable<bool> UseAvatarLogoAsDefault
        {
            get => useAvatarLogoAsDefault;
            set
            {
                useAvatarLogoAsDefault?.UnbindAll();
                useAvatarLogoAsDefault = value;

                value.BindValueChanged(_ =>
                {
                    playerProperties.Metadata["mpris:artUrl"] = resolveBeatmapCoverUrl(beatmap);
                    OnPropertiesChanged?.Invoke(PropertyChanges.ForProperty("Metadata", playerProperties.Metadata));
                }, true);
            }
        }

        private string resolveBeatmapCoverUrl(WorkingBeatmap beatmap)
        {
            string body;
            string backgroundFilename = beatmap?.BeatmapInfo.Metadata.BackgroundFile;

            if (!string.IsNullOrEmpty(backgroundFilename) && !(UseAvatarLogoAsDefault?.Value ?? false))
            {
                body = Storage?.GetFullPath("files")
                       + Path.DirectorySeparatorChar
                       + (beatmap.BeatmapSetInfo.GetPathForFile(beatmap.BeatmapInfo.Metadata?.BackgroundFile)
                          ?? string.Empty);

                Logging.Log("COVER PATH IS " + body);
            }
            else
            {
                string target = Storage?.GetFiles("custom", "avatarlogo*")
                                       .FirstOrDefault(s => s.Contains("avatarlogo"));

                if (!string.IsNullOrEmpty(target))
                    body = Storage.GetFullPath(target);
                else
                    return string.Empty;
            }

            return $"file://{body}";
        }

        public Task RunIfAudioControlEnabled(Action action)
        {
            if (AudioControlDisabled)
                return Task.CompletedTask;

            action();

            return Task.CompletedTask;
        }

        #region 实现Mpris接口

        public Task NextAsync()
        {
            return RunIfAudioControlEnabled(() => Next?.Invoke());
        }

        public Task PreviousAsync()
        {
            return RunIfAudioControlEnabled(() => Previous?.Invoke());
        }

        public Task PauseAsync()
        {
            return RunIfAudioControlEnabled(() => Pause?.Invoke());
        }

        public Task PlayPauseAsync()
        {
            return RunIfAudioControlEnabled(() => Play?.Invoke());
        }

        public Task StopAsync()
        {
            return RunIfAudioControlEnabled(() => Stop?.Invoke());
        }

        public Task PlayAsync()
        {
            return RunIfAudioControlEnabled(() =>
            {
                Play?.Invoke();
                playerProperties._PlaybackStatus = MprisStatusStrings.PLAYBACK_PLAYING;
            });
        }

        public Task SeekAsync(long offset)
        {
            return RunIfAudioControlEnabled(() => Seek?.Invoke(offset));
        }

        public Task SetPositionAsync(ObjectPath trackId, long position)
        {
            return RunIfAudioControlEnabled(() =>
            {
                SetPosition?.Invoke(position);
            });
        }

        public Task OpenUriAsync(string uri)
        {
            OpenUri?.Invoke(uri);
            return Task.CompletedTask;
        }

        public event Action<long> OnSeeked;

        public void InvokeSeeked(long position)
        {
            OnSeeked?.Invoke(position);
        }

        public Task<IDisposable> WatchSeekedAsync(Action<long> handler, Action<Exception> onError = null)
            => SignalWatcher.AddAsync(this, nameof(OnSeeked), handler);

        public Task RaiseAsync()
        {
            WindowRaise?.Invoke();
            return Task.CompletedTask;
        }

        public Task QuitAsync()
        {
            Quit?.Invoke();
            return Task.CompletedTask;
        }

        #endregion

        public Task<object> GetAsync(string prop)
            => Task.FromResult(get(prop));

        private object get(string prop)
        {
            return mp2Properties.Contains(prop)
                ? mp2Properties.Get(prop)
                : playerProperties.Get(prop);
        }

        public Task SetAsync(string target, object val)
        {
            //暂时不接受外部设置
            //'Set(target, val, true);

            return Task.CompletedTask;
        }

        internal void TriggerPropertyChangeFor(string target)
        {
            object value = get(target);

            OnPropertiesChanged?.Invoke(PropertyChanges.ForProperty(target, value));
        }
/*
        internal void Set(string target, object val, bool isExternal = false)
        {
            if (!AllowSet) return;

            // todo: Implement this to allow external set?
            if (playerProperties.Set(target, val, isExternal))
            {
                //bool abortInvoke = false;

                if (isExternal)
                {
                    switch (target)
                    {
                        case nameof(playerProperties.Volume):
                            OnVolumeSet?.Invoke(playerProperties.Volume);
                            break;

                        case nameof(playerProperties.LoopStatus):
                            if (val is string loopStatus)
                                LoopChange?.Invoke(loopStatus != MprisStatusStrings.LOOP_STATUS_SINGLE);
                            break;

                        case nameof(playerProperties.Shuffle):
                            if (val is bool v)
                                OnRandom?.Invoke(v);

                            break;
                    }
                }

                //if (!abortInvoke)
                OnPropertiesChanged?.Invoke(PropertyChanges.ForProperty(target, val));
            }
        }
*/
        Task<MediaPlayer2Properties> IMediaPlayer2.GetAllAsync()
            => Task.FromResult(mp2Properties);

        public Task<PlayerProperties> GetAllAsync()
            => Task.FromResult(playerProperties);

        public event Action<PropertyChanges> OnPropertiesChanged;

        public Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler)
            => SignalWatcher.AddAsync(this, nameof(OnPropertiesChanged), handler);
    }
}
