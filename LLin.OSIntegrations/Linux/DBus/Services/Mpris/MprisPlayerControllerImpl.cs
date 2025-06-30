using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;
using Tmds.DBus.SourceGenerator;

namespace LLin.OSIntegrations.Linux.DBus.Services.Mpris;

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
        LoopStatusInternal = MprisStatusStrings.LOOP_STATUS_NONE;

        Rate = 1d;
        MaximumRate = 1d;
        MinimumRate = 1d;

        ShuffleInternal = false;
        Volume = 1d;
        Position = 0L;

        CanGoNext = true;
        CanGoPrevious = true;
        CanPlay = true;
        CanPause = true;
        CanSeek = true;
        CanControl = true;
    }

    internal static readonly string[] EMPTY_STRING_ARRAY = [""];

    protected void NotifyChange<T>(string name, T val)
        where T : notnull
    {
        Dictionary<string, VariantValue> dict = new Dictionary<string, VariantValue>();

        switch (val)
        {
            case Dictionary<string, VariantValue> d:
            {
                var dbusDict = new Dict<string, VariantValue>(d);
                dict[name] = dbusDict.AsVariantValue();
                break;
            }

            case string s:
            {
                dict[name] = VariantValue.String(s);
                break;
            }

            case short ss:
            {
                dict[name] = VariantValue.Int16(ss);
                break;
            }

            case int i:
            {
                dict[name] = VariantValue.Int32(i);
                break;
            }

            case long l:
            {
                dict[name] = VariantValue.Int64(l);
                break;
            }

            case float f:
            {
                dict[name] = VariantValue.Double(f);
                break;
            }

            case double d:
            {
                dict[name] = VariantValue.Double(d);
                break;
            }

            case bool b:
            {
                dict[name] = VariantValue.Bool(b);
                break;
            }

            case byte bb:
            {
                dict[name] = VariantValue.Byte(bb);
                break;
            }

            default:
            {
                throw new NotSupportedException($"{val.GetType()} is not supported yet, this might be a bug!");
            }
        }

        MessageWriter writer = Connection.GetMessageWriter();
        writer.WriteSignalHeader(null, "/org/mpris/MediaPlayer2", "org.freedesktop.DBus.Properties", "PropertiesChanged", "sa{sv}as");
        writer.WriteString("org.mpris.MediaPlayer2.Player");
        writer.WriteDictionary(dict);
        writer.WriteArray(EMPTY_STRING_ARRAY);

        if (!Connection.TrySendMessage(writer.CreateMessage()))
        {
            //throw new Exception("Can't send notify!");
        }

        writer.Dispose();
    }

    //region OrgMprisMediaPlayer2PlayerHandler

    public override Connection Connection { get; }

    private string? loopStatus = MprisStatusStrings.LOOP_STATUS_NONE;

    /// <summary>
    /// To set loop status, use <see cref="LoopStatusInternal"/>
    /// </summary>
    [Obsolete]
    public override string? LoopStatus
    {
        get => loopStatus;
        set
        {
            // Ignore as we don't support setting loop status from media integration
        }
    }

    public string? LoopStatusInternal
    {
        get => loopStatus;
        set
        {
            loopStatus = value;

            if (value != null)
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

    /// <summary>
    /// To set shuffle status, use <see cref="ShuffleInternal"/>
    /// </summary>
    [Obsolete]
    public override bool Shuffle
    {
        get => shuffle;
        set
        {
            shuffle = value;
            ShuffleChanged?.Invoke(value);
        }
    }

    public bool ShuffleInternal
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

    public new Dictionary<string, VariantValue>? Metadata
    {
        get => base.Metadata;
        set
        {
            base.Metadata = value;

            if (value != null)
                NotifyChange(nameof(Metadata), value);
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

    public event Action<bool>? ShuffleChanged;

    public event Action<long>? Seek;
    public event Action<long>? SetPosition;
    public event Action<string>? OpenUri;

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
