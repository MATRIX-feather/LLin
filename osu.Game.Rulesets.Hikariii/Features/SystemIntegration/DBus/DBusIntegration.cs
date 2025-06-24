using System;
using M.DBus;
using M.DBus.Services;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;

namespace osu.Game.Rulesets.Hikariii.Features.SystemIntegration.DBus;

public partial class DBusIntegration : CompositeComponent
{
    public readonly DBusManager<IMDBusObject> DBusManager;
    public readonly Greet GreetService;

    public DBusIntegration()
    {
        this.DBusManager = new DBusManager<IMDBusObject>();
        DBusManager.OnConnected += onConnected;

        GreetService = new Greet("hikariii");
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        DBusManager.Connect().Wait();

        Logging.Log("Done connecting to DBus!");
    }

    public Action<DBusManager<IMDBusObject>>? OnDBusConnected;

    private void onConnected()
    {
        DBusManager.RegisterNewObject(GreetService).Wait();
        GreetService.SwitchState(true, "DBus connected");

        OnDBusConnected?.Invoke(DBusManager);
    }
}
