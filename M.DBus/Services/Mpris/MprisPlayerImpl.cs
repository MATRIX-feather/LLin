using System.Threading.Tasks;
using Tmds.DBus.Protocol;
using Tmds.DBus.SourceGenerator;

namespace M.DBus.Services.Mpris;

internal class MprisPlayerImpl : OrgMprisMediaPlayer2Handler, IMDBusObject
{
    public static readonly string PATH = "/org/mpris/MprisPlayer2";

    public MprisPlayerImpl(Connection bindingConnection)
    {
        Connection = bindingConnection;
        this.PathHandler = new PathHandler(Path);

        Setup();
    }

    public void Setup()
    {
        CanQuit = false;
        Fullscreen = false;
        CanSetFullscreen = false;
        CanRaise = true;
        HasTrackList = false;
        Identity = "osu!";
        DesktopEntry = "osu!";
        SupportedMimeTypes = [];
        SupportedUriSchemes = [];
    }

    public override Connection Connection { get; }
    public override bool Fullscreen { get; set; } = false;

    protected override ValueTask OnRaiseAsync(Message request)
    {
        return ValueTask.CompletedTask;
    }

    protected override ValueTask OnQuitAsync(Message request)
    {
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

    public string Path { get; } = PATH;
}
