using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Mpris;
using Tmds.DBus.Protocol;
using Tmds.DBus.SourceGenerator;

namespace M.DBus.Services.Mpris;

public class MprisService : IMethodHandler
{
    internal readonly MprisPlayerImpl MprisPlayerService;
    internal readonly MprisPlayerControllerImpl MprisPlayerControllerService;

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

    public Dictionary<string, Variant>? Metadata
    {
        get => MprisPlayerControllerService.Metadata;
        set => MprisPlayerControllerService.Metadata = value;
    }

    public long Progress
    {
        set => MprisPlayerControllerService.Position = value;
    }

    public long TrackLength
    {
        set
        {
            var metadata = MprisPlayerControllerService.Metadata;
            metadata["mpris:length"] = value;

            MprisPlayerControllerService.Metadata = metadata;
        }
    }

    public bool TrackRunning
    {
        set => MprisPlayerControllerService.PlaybackStatus = value ? "Playing" : "Paused";
    }

    public bool TrackLooping
    {
        set => MprisPlayerControllerService.LoopStatusInternal = value ? MprisStatusStrings.LOOP_STATUS_SINGLE : MprisStatusStrings.LOOP_STATUS_NONE;
    }

    public bool Shuffle
    {
        set => MprisPlayerControllerService.ShuffleInternal = value;
    }

    public bool AllowExternalControl
    {
        set
        {
            MprisPlayerControllerService.CanControl = value;
            MprisPlayerControllerService.CanSeek = value;
            MprisPlayerControllerService.CanPause = value;
            MprisPlayerControllerService.CanGoNext = value;
            MprisPlayerControllerService.CanGoPrevious = value;
            MprisPlayerControllerService.CanPlay = value;
        }
    }

    private PathHandler PathHandler;

    public MprisService(Connection dbusProtocolConnection)
    {
        MprisPlayerService = new MprisPlayerImpl(dbusProtocolConnection);
        MprisPlayerControllerService = new MprisPlayerControllerImpl(dbusProtocolConnection);

        PathHandler = new PathHandler(this.Path);
        PathHandler.Add(MprisPlayerControllerService);
        PathHandler.Add(MprisPlayerService);

        MprisPlayerControllerService.Next += () => Next?.Invoke();
        MprisPlayerControllerService.Previous += () => Previous?.Invoke();
        MprisPlayerControllerService.Pause += () => Pause?.Invoke();
        MprisPlayerControllerService.Stop += () => Stop?.Invoke();
        MprisPlayerControllerService.Play += () => Play?.Invoke();
        MprisPlayerControllerService.Seek += offset => Seek?.Invoke(offset);
        MprisPlayerControllerService.SetPosition += pos => SetPosition?.Invoke(pos);
        MprisPlayerControllerService.OpenUri += uri => OpenUri?.Invoke(uri);
        MprisPlayerControllerService.PlayPause += () => TogglePause?.Invoke();

        MprisPlayerControllerService.ShuffleChanged += v => ShuffleChanged?.Invoke(v);
    }

    public void Register(Connection connection)
    {
        connection.AddMethodHandlers([this]);
    }

    public ValueTask HandleMethodAsync(MethodContext context)
    {
        return PathHandler.HandleMethodAsync(context);
    }

    public bool RunMethodHandlerSynchronously(Message message)
    {
        return PathHandler.RunMethodHandlerSynchronously(message);
    }

    public string Path { get; } = "/org/mpris/MediaPlayer2";
}
