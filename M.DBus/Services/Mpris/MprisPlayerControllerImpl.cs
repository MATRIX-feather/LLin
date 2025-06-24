using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Mpris;
using Tmds.DBus.Protocol;
using Tmds.DBus.SourceGenerator;

namespace M.DBus.Services.Mpris;

internal partial class MprisPlayerControllerImpl : OrgMprisMediaPlayer2PlayerHandler
{
    public MprisPlayerControllerImpl(Connection bindingConnection)
    {
        Connection = bindingConnection;
        Setup();
    }

    public void Setup()
    {
        Metadata = new();

        PlaybackStatus = MprisStatusStrings.PLAYBACK_PLAYING;
        LoopStatus = MprisStatusStrings.LOOP_STATUS_NONE;

        Rate = 1d;
        MaximumRate = 1d;
        MinimumRate = 1d;

        Shuffle = false;
        Volume = 1d;
        Position = 0L;

        CanGoNext = true;
        CanGoPrevious = true;
        CanPlay = true;
        CanPause = true;
        CanSeek = true;
        CanControl = true;
    }

    public string Path { get; } = "/org/mpris/MediaPlayer2";

    protected void NotifyChange<T>(string name, T val)
    {
        Dictionary<string, Variant> dict;

        if (val is Dictionary<string, Variant> d)
        {
            dict = d;
        }
        else
        {
            dict = new Dictionary<string, Variant>
            {
                [name] = Variant.FromStruct(new Struct<T>(val))
            };
        }

        MessageWriter writer = Connection.GetMessageWriter();
        writer.WriteSignalHeader(null, Path, "org.freedesktop.DBus.Properties", "PropertiesChanged", "sa{sv}as");
        writer.WriteString("org.mpris.MediaPlayer2.Player");
        writer.WriteDictionary(dict);
        writer.WriteArray(new[] { "" });

        if (!Connection.TrySendMessage(writer.CreateMessage()))
            throw new Exception("Can't send notify!");

        writer.Dispose();
    }

    //region OrgMprisMediaPlayer2PlayerHandler

    public override Connection Connection { get; }

    private string? loopStatus = MprisStatusStrings.LOOP_STATUS_NONE;

    public override string? LoopStatus
    {
        get => loopStatus;
        set
        {
            loopStatus = value;
            NotifyChange(nameof(LoopStatus), value);
        }
    }

    private double rate = 1d;

    public override double Rate
    {
        get => rate;
        set
        {
            rate = value;
            NotifyChange(nameof(Rate), value);
        }
    }

    private bool shuffle;

    public override bool Shuffle
    {
        get => shuffle;
        set
        {
            shuffle = value;
            NotifyChange(nameof(Shuffle), value);
        }
    }

    private double volume = 1d;

    public override double Volume
    {
        get => volume;
        set
        {
            volume = value;
            NotifyChange(nameof(Volume), value);
        }
    }

    public new double MaximumRate
    {
        get => base.MaximumRate;
        set
        {
            base.MaximumRate = value;
            NotifyChange(nameof(MaximumRate), value);
        }
    }

    public new double MinimumRate
    {
        get => base.MinimumRate;
        set
        {
            base.MinimumRate = value;
            NotifyChange(nameof(MinimumRate), value);
        }
    }

    public new long Position
    {
        get => base.Position;
        set
        {
            base.Position = value;
            NotifyChange(nameof(Position), value);
        }
    }

    public new Dictionary<string, Variant>? Metadata
    {
        get => base.Metadata;
        set
        {
            base.Metadata = value;
            NotifyChange(nameof(Metadata), Metadata);
        }
    }

    public new bool CanGoNext
    {
        get => base.CanGoNext;
        set
        {
            base.CanGoNext = value;
            NotifyChange(nameof(CanGoNext), value);
        }
    }

    public new bool CanGoPrevious
    {
        get => base.CanGoPrevious;
        set
        {
            base.CanGoPrevious = value;
            NotifyChange(nameof(CanGoPrevious), value);
        }
    }

    public new bool CanPlay
    {
        get => base.CanPlay;
        set
        {
            base.CanPlay = value;
            NotifyChange(nameof(CanPlay), value);
        }
    }

    public new bool CanPause
    {
        get => base.CanPause;
        set
        {
            base.CanPause = value;
            NotifyChange(nameof(CanPause), value);
        }
    }

    public new bool CanSeek
    {
        get => base.CanSeek;
        set
        {
            base.CanSeek = value;
            NotifyChange(nameof(CanSeek), value);
        }
    }

    public new bool CanControl
    {
        get => base.CanControl;
        set
        {
            base.CanControl = value;
            NotifyChange(nameof(CanControl), CanControl);
        }
    }

    public event Action? Next;
    public event Action? Previous;
    public event Action? Pause;
    public event Action? Stop;
    public event Action? Play;
    public event Action? PlayPause;

    public event Action<long>? Seek;
    public event Action<long>? SetPosition;
    public event Action<string>? OpenUri;

    public static readonly string PATH = "/org/mpris/MediaPlayer2";

    protected override ValueTask OnNextAsync(Message request)
    {
        Next?.Invoke();
        return ValueTask.CompletedTask;
    }

    protected override ValueTask OnPreviousAsync(Message request)
    {
        Previous?.Invoke();
        return ValueTask.CompletedTask;
    }

    protected override ValueTask OnPauseAsync(Message request)
    {
        Pause?.Invoke();
        return ValueTask.CompletedTask;
    }

    protected override ValueTask OnPlayPauseAsync(Message request)
    {
        PlayPause?.Invoke();
        return ValueTask.CompletedTask;
    }

    protected override ValueTask OnStopAsync(Message request)
    {
        Stop?.Invoke();
        return ValueTask.CompletedTask;
    }

    protected override ValueTask OnPlayAsync(Message request)
    {
        Play?.Invoke();
        return ValueTask.CompletedTask;
    }

    protected override ValueTask OnSeekAsync(Message request, long offset)
    {
        Seek?.Invoke(offset);
        return ValueTask.CompletedTask;
    }

    protected override ValueTask OnSetPositionAsync(Message request, ObjectPath trackId, long position)
    {
        SetPosition?.Invoke(position);
        return ValueTask.CompletedTask;
    }

    protected override ValueTask OnOpenUriAsync(Message request, string uri)
    {
        OpenUri?.Invoke(uri);
        return ValueTask.CompletedTask;
    }
    //endregion
}
