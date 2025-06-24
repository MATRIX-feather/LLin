using System;
using M.DBus;
using M.DBus.Services;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using Tmds.DBus.Protocol;

namespace osu.Game.Rulesets.Hikariii.Features.SystemIntegration.DBus;

public partial class DBusIntegration : CompositeComponent
{
    public DBusIntegration()
    {
    }

    public DBusSession AcquireNewSession()
    {
        return new DBusSession(Address.Session!);
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        int start = DateTime.Now.Millisecond;

        var session = AcquireNewSession();
        session.OnConnected += registerGreeter;
        session.Connect().Wait();

        int end = DateTime.Now.Millisecond;

        Logging.Log($"Done connecting to DBus! Took {end - start}ms.");
    }

    private void registerGreeter(DBusSession session)
    {
        if (session == null)
            throw new ArgumentNullException(nameof(session));

        session.RequestServiceName("xyz.nifeather.mfosu").Wait();
        var connection = session.CurrentConnection!;

        var greeter = new GreeterService();
        greeter.register(connection);
    }

    //public Action<DBusMgrNew>? OnDBusConnected;

    private void onConnected()
    {
        //GreetService.register(DBusManager.CurrentConnection);
        //DBusManager.RegisterObject(GreetService).Wait();
        //GreetService.SwitchState(true, "DBus connected");

        //OnDBusConnected?.Invoke(DBusManager);
    }
}
