using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using M.DBus.Services;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;
using Tmds.DBus.Protocol;

namespace M.DBus;

#nullable enable

public partial class DBusMgrNew : CompositeDrawable
{
    public Connection? CurrentConnection { get; private set; }

    public DBusAccess? CurrentAccess { get; private set; }

    public string TargetUrl { get; set; } = Address.Session;

    #region Connect and Disconnect

    private CancellationTokenSource? cancellationTokenSource;

    public Task Connect()
    {
        Disconnect();

        this.cancellationTokenSource = new CancellationTokenSource();
        CurrentConnection = new Connection(new ClientConnectionOptions(TargetUrl)
        {
            AutoConnect = false
        });

        return Task.Run(startConnectTask, cancellationTokenSource.Token);
    }

    private Task startConnectTask()
    {
        try
        {
            if (CurrentConnection == null)
                throw new NullDependencyException("Called StartConnect but DBusConnection is not ready!");

            CurrentConnection = new Connection(TargetUrl);

            // Await for connection to finish
            //currentConnection.StateChanged += onConnectionStateChanged;

            CurrentConnection.ConnectAsync().AsTask().Wait();
            this.CurrentAccess = new DBusAccess(new DBusProxy(CurrentConnection));

            CurrentAccess.RequestName("aaaio.matrix_feather.mfosu", 0).Wait();
            Logger.Log("Connected to DBus!");

            OnConnected?.Invoke();

            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            Logger.Error(e, "初始化到DBus的连接时出现异常");

            return Task.FromException(e);
        }
    }
/*
    private void onConnectionStateChanged(object? sender, ConnectionStateChangedEventArgs e)
    {
        Logger.Log($"-----------------------DBus Connection State Changed--------------------------");

        Logger.Log($"Sender: {sender}");
        Logger.Log($"Connection State: {e.State}");

        if (e.DisconnectReason != null)
            Logger.Log($"Reason: {e.DisconnectReason}");

        if (e.DisconnectReason != null)
            Logger.Log($"StackTrace: {e.DisconnectReason.StackTrace}");

        if (e.State == ConnectionState.Connected)
        {
            Logger.Log($"Info#LocalName: {e.ConnectionInfo.LocalName}");
            Logger.Log($"Info#RemoteIsBus: {e.ConnectionInfo.RemoteIsBus}");
        }

        Logger.Log($"-----------------------End State Report---------------------------------------");

        this.ConnectionState = e.State;
    }
*/
    public void Disconnect()
    {
        cancellationTokenSource?.Cancel();

        CurrentConnection?.Dispose();
        CurrentConnection = null;
    }

    #endregion

    #region Object Register

    private readonly ConcurrentDictionary<IMDBusObject, string> registedObjects = new();

    /// <summary>
    /// Register a dbus object to this manager.
    /// </summary>
    /// <param name="dBusObject">The target object to register</param>
    /// <returns></returns>
    public Task RegisterObject(IMDBusObject? dBusObject)
    {
        if (dBusObject == null)
            return Task.FromException(new Exception("Null DBusObject!"));

        if (registedObjects.ContainsKey(dBusObject))
            return Task.FromException(new Exception("Already have an object registered in the same path!"));

        registedObjects[dBusObject] = dBusObject.Path;

        return Task.Run(() => registerToConnectionTask(dBusObject));
    }

    /// <summary>
    /// 将目标对象注册到DBus连接上
    /// <br/>
    /// 如果连接没准备好，则不会做任何事
    /// </summary>
    private async Task registerToConnectionTask(IMDBusObject obj)
    {
        if (!ConnectionReady())
            throw new Exception("Connection not ready!");

        Debug.Assert(CurrentConnection != null, nameof(CurrentConnection) + " != null");
        CurrentConnection.AddMethodHandler(obj);
    }

    /// <summary>
    /// 从当前连接移除DBus对象
    /// </summary>
    /// <param name="mdBusObject">要移除的对象</param>
    /// <param name="failSoft">对象不在注册表中时返回<see cref="RegisterResult.FAILED"/>而不是抛出异常</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">给定的对象不在注册表中</exception>
    public RegisterResult UnRegisterObject(IMDBusObject? mdBusObject, bool failSoft = false)
    {
        Logger.Log($"Unregister object: {mdBusObject}", level: LogLevel.Debug);

        if (mdBusObject == null)
            return RegisterResult.NULL_OBJECT;

        if (!registedObjects.ContainsKey(mdBusObject))
            return RegisterResult.NO_SUCH_OBJECT;

        string? srvName = registedObjects.GetValueOrDefault(mdBusObject);

        if (!registedObjects.TryRemove(mdBusObject, out _))
        {
            if (failSoft) return RegisterResult.FAILED;

            throw new InvalidOperationException($"The given object ({mdBusObject}) does not exist in the current registry");
        }

        this.CurrentConnection?.RemoveMethodHandler(mdBusObject.Path);

        Task.Run(() => unRegisterFromConnectionTask(mdBusObject, srvName!));
        return RegisterResult.OK;
    }

    /// <summary>
    /// 将目标对象从DBus连接上移除
    /// <br/>
    /// 如果连接没准备好，则不会做任何事
    /// </summary>
    private async Task unRegisterFromConnectionTask(IMDBusObject mdBusObject, string serviceName)
    {
        if (!ConnectionReady()) return;

        this.CurrentConnection!.RemoveMethodHandler(mdBusObject.Path);
    }

    #endregion

    public Action? OnConnected;

    public bool ConnectionReady()
    {
        return this.CurrentConnection != null;
    }

    protected override void Dispose(bool isDisposing)
    {
        this.Disconnect();

        base.Dispose(isDisposing);
    }

    public enum RegisterResult
    {
        OK,
        FAILED,
        PATH_ALREADY_IN_USE,
        NULL_OBJECT,
        NO_SUCH_OBJECT
    }
}
