using System;
using M.DBus;
using M.DBus.Services;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;

namespace osu.Game.Rulesets.Hikariii.Features.SystemIntegration.DBus;

public partial class DBusIntegration : CompositeComponent
{
    public readonly DBusMgrNew DBusManager;
    public readonly Greet GreetService;

    public DBusIntegration()
    {
        this.DBusManager = new DBusMgrNew();
        DBusManager.OnConnected += onConnected;

        GreetService = new Greet("hikariii");
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        int start = DateTime.Now.Millisecond;
        DBusManager.Connect().Wait();
        int end = DateTime.Now.Millisecond;

        Logging.Log($"Done connecting to DBus! Took {end - start}ms.");
    }

    public Action<DBusMgrNew>? OnDBusConnected;

    private void onConnected()
    {
        DBusManager.RegisterObject(GreetService).Wait();
        GreetService.SwitchState(true, "DBus connected");

        OnDBusConnected?.Invoke(DBusManager);
    }
}
