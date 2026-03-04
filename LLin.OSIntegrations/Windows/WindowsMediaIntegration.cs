using System;

#if WINDOWS
using Windows.Media;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;
#endif

namespace LLin.OSIntegrations.Windows;

public class WindowsMediaIntegration
{
#if WINDOWS
    private readonly MediaPlayer media;
    private readonly SystemMediaTransportControls smtc;

    private void updateDisplay(Action<SystemMediaTransportControlsDisplayUpdater> action)
    {
        action(smtc.DisplayUpdater);
        smtc.DisplayUpdater.Update();
    }

    public string Title
    {
        set => updateDisplay(display => display.MusicProperties.Title = value);
    }

    public string Artist
    {
        set => updateDisplay(display => display.MusicProperties.Artist = value);
    }

    public string Album
    {
        set => updateDisplay(display => display.MusicProperties.AlbumTitle = value);
    }

    private double lastValidPositionMillisecond;

    public double Progress
    {
        set
        {
            lastValidPositionMillisecond = value;
            media.Position = TimeSpan.FromMilliseconds(value);

            updateTimelineProperties();
        }
    }

    private double lastValidTrackLengthMillisecond;

    public double TrackLength
    {
        set
        {
            lastValidTrackLengthMillisecond = value;
            updateTimelineProperties();
        }
    }

    private readonly SystemMediaTransportControlsTimelineProperties timelineProperties = new();

    private void updateTimelineProperties()
    {
        timelineProperties.EndTime = TimeSpan.FromMilliseconds(lastValidTrackLengthMillisecond);
        timelineProperties.StartTime = TimeSpan.FromMilliseconds(0);
        timelineProperties.Position = TimeSpan.FromMilliseconds(lastValidPositionMillisecond);

        timelineProperties.MinSeekTime = TimeSpan.FromMilliseconds(0);
        timelineProperties.MaxSeekTime = TimeSpan.FromMilliseconds(lastValidTrackLengthMillisecond);

        smtc.UpdateTimelineProperties(timelineProperties);
    }

    public string CoverPath
    {
        set
        {
            if (string.IsNullOrEmpty(value))
                return;

            updateDisplay(display =>
            {
                var storageFile = StorageFile.GetFileFromPathAsync(value).GetAwaiter().GetResult();

                if (storageFile != null)
                    display.Thumbnail = RandomAccessStreamReference.CreateFromFile(storageFile);
            });
        }
    }

    public bool TrackRunning
    {
        set => smtc.PlaybackStatus = value ? MediaPlaybackStatus.Playing : MediaPlaybackStatus.Paused;
    }

    public bool TrackLooping
    {
        set => smtc.AutoRepeatMode = value ? MediaPlaybackAutoRepeatMode.Track : MediaPlaybackAutoRepeatMode.None;
    }

    public bool Shuffle
    {
        set => smtc.ShuffleEnabled = value;
    }

    public bool AllowExternalControl
    {
        // set => smtc.IsEnabled = value;
        get; set;
    }

    //endregion Controls

    public WindowsMediaIntegration()
    {
        this.media = new MediaPlayer();
        this.smtc = media.SystemMediaTransportControls;

        media.CommandManager.IsEnabled = false;
        media.PlaybackRate = 1d;
        media.PlaybackSession.Position = TimeSpan.FromSeconds(10);

        smtc.IsEnabled = true;
        smtc.DisplayUpdater.Type = MediaPlaybackType.Music;
        smtc.DisplayUpdater.Update();

        smtc.IsPlayEnabled = true;
        smtc.IsPauseEnabled = true;
        smtc.IsNextEnabled = true;
        smtc.IsPreviousEnabled = true;
        smtc.IsStopEnabled = true;

        smtc.ButtonPressed += smtcButtonPressed;
        smtc.PropertyChanged += smtcPropertyChanged;
        smtc.PlaybackPositionChangeRequested += smtcPlaybackPositionChangeRequested;

        smtc.PlaybackRate = 1d;
        smtc.PlaybackRateChangeRequested += smtcPlaybackRateChangeRequested;
    }

    private void smtcPlaybackRateChangeRequested(SystemMediaTransportControls sender,
                                                 PlaybackRateChangeRequestedEventArgs args)
    {
    }

    private void smtcPlaybackPositionChangeRequested(SystemMediaTransportControls sender,
                                                     PlaybackPositionChangeRequestedEventArgs args)
    {
        SetPosition?.Invoke(args.RequestedPlaybackPosition.Milliseconds);
    }

    private void smtcPropertyChanged(SystemMediaTransportControls sender,
                                     SystemMediaTransportControlsPropertyChangedEventArgs args)
    {
        // do nothing
    }

    private void smtcButtonPressed(SystemMediaTransportControls sender,
                                   SystemMediaTransportControlsButtonPressedEventArgs args)
    {
        switch (args.Button)
        {
            case SystemMediaTransportControlsButton.Play:
                Play?.Invoke();
                break;

            case SystemMediaTransportControlsButton.Pause:
                Pause?.Invoke();
                break;

            case SystemMediaTransportControlsButton.Stop:
                Stop?.Invoke();
                break;

            case SystemMediaTransportControlsButton.Next:
                Next?.Invoke();
                break;

            case SystemMediaTransportControlsButton.Previous:
                Previous?.Invoke();
                break;

            case SystemMediaTransportControlsButton.FastForward:
                Seek?.Invoke(10000L); //10000ms -> 1s
                break;

            case SystemMediaTransportControlsButton.Rewind:
                Seek?.Invoke(-10000L); // 10000ms -> 10s
                break;

            case SystemMediaTransportControlsButton.Record:
            case SystemMediaTransportControlsButton.ChannelUp:
            case SystemMediaTransportControlsButton.ChannelDown:
            default:
                // Ignore these buttons
                break;
        }
    }
#endif

    //region Controls

    public event Action? Next;
    public event Action? Previous;
    public event Action? Pause;
    public event Action? Stop;
    public event Action? Play;
    public event Action? TogglePause;

    public event Action<bool>? ShuffleChanged;

    public event Action<long>? Seek;
    public event Action<long>? SetPosition;
    public event Action<string>? OpenUri;
}
