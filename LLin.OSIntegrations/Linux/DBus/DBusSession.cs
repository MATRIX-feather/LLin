using System;
using System.Threading.Tasks;
using LLin.OSIntegrations.Linux.DBus.Services.DBusAccess;
using osu.Framework.Logging;
using Tmds.DBus.Protocol;

namespace LLin.OSIntegrations.Linux.DBus;

public class DBusSession(string address)
{
    public Connection? CurrentConnection { get; private set; }
    public DBusAccess? DBusAccess { get; private set; }

    public readonly string TargetAddress = address;

    public event Action<DBusSession>? OnConnected;

    public Task Connect()
    {
        var connection = new Connection(TargetAddress);
        var connectTask = connection.ConnectAsync();
        connectTask.AsTask().Wait();

        if (!connectTask.IsCompletedSuccessfully)
        {
            var taskException = connectTask.AsTask().Exception;
            return Task.FromException(taskException ?? new Exception("Connect failed for unknown reason."));
        }

        CurrentConnection = connection;
        DBusAccess = new DBusAccess(new DBusProxy(connection));

        OnConnected?.Invoke(this);

        Logger.Log("Successfully connected to DBus!");

        return Task.CompletedTask;
    }

    public Task RequestServiceName(string serviceName)
    {
        return DBusAccess?.RequestName(serviceName, 0)
               ?? Task.FromException(new Exception("DBus access not initialized."));
    }
}
