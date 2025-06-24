using System;
using System.Threading.Tasks;
using osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Mpris;
using Tmds.DBus.Protocol;
using Tmds.DBus.SourceGenerator;

namespace M.DBus.Services.Mpris;

internal partial class MprisPlayerControllerImpl : OrgMprisMediaPlayer2PlayerHandler, IMDBusObject
{
    public MprisPlayerControllerImpl(Connection bindingConnection)
    {
        Connection = bindingConnection;
        this.PathHandler = new PathHandler(Path);
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

    //region OrgMprisMediaPlayer2PlayerHandler

    public override Connection Connection { get; }
    public override string? LoopStatus { get; set; } = MprisStatusStrings.LOOP_STATUS_NONE;
    public override double Rate { get; set; } = 1;
    public override bool Shuffle { get; set; } = false;
    public override double Volume { get; set; } = 1;

    public event Action? Next;
    public event Action? Previous;
    public event Action? Pause;
    public event Action? Stop;
    public event Action? Play;

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
        Play?.Invoke();
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

    public ValueTask HandleMethodAsync(MethodContext context)
    {
        return PathHandler!.HandleMethodAsync(context);
    }

    public bool RunMethodHandlerSynchronously(Message message)
    {
        return PathHandler!.RunMethodHandlerSynchronously(message);
    }

    //endregion

    public string Path { get; } = PATH;
}
