#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using M.DBus.Services;
using M.DBus.Tray;
using M.DBus.Utils;
using osu.Framework.Logging;
using Tmds.DBus;

namespace M.DBus
{
    public class DBusManager : DBusManager<IDBusObject>
    {
        public DBusManager(bool startOnLoad, IHandleTrayManagement trayManagement, IHandleSystemNotifications systemNotifications)
            : base(startOnLoad, trayManagement, systemNotifications)
        {
        }
    }

    public class DBusManager<T> : IDisposable
        where T : IDBusObject
    {
        public Action OnConnected;

        public Greet GreetService = new Greet();
        private Connection currentConnection;

        private ConnectionState connectionState = ConnectionState.NotConnected;

        private bool isDisposed { get; set; }

        public readonly IHandleTrayManagement TrayManager;

        public readonly IHandleSystemNotifications Notifications;

        public DBusManager(bool startOnLoad, IHandleTrayManagement trayManagement, IHandleSystemNotifications systemNotifications)
        {
            //如果在初始化时启动服务
            if (startOnLoad)
                Connect();

            TrayManager = trayManagement;
            Notifications = systemNotifications;

            Task.Run(() => RegisterNewObject(GreetService));
        }

        #region Disposal

        public void Dispose()
        {
            //Disconnect();

            currentConnection.Dispose();

            isDisposed = true;
            GC.SuppressFinalize(this);
        }

        #endregion

        #region 工具

        public async Task GetAllServices()
        {
            await currentConnection.ListServicesAsync().ConfigureAwait(false);
            string[] services = await currentConnection.ListServicesAsync().ConfigureAwait(false);

            foreach (string service in services) Logger.Log(service);
        }

        private void onServiceNameChanged(ServiceOwnerChangedEventArgs args)
        {
            Logger.Log($"服务 '{args.ServiceName}' 的归属现在从 '{args.OldOwner}' 变为 '{args.NewOwner}'");
        }

        private void onServiceError(Exception e, IDBusObject dbusObject)
        {
            connectionState = ConnectionState.Faulted;

            Logger.Error(e, $"位于 '{dbusObject.ObjectPath.ToServiceName()}' 的DBus服务出现错误");
        }

        public bool CheckIfAlreadyRegistered(IDBusObject dBusObject)
        {
            return registerDictionary.Any(o => o.Key.ObjectPath.Equals(dBusObject.ObjectPath));
        }

        public bool CheckIfAlreadyRegistered(ObjectPath objectPath)
        {
            return registerDictionary.Any(o => o.Key.ObjectPath.Equals(objectPath));
        }

        public S GetDBusObject<S>(ObjectPath path, string name = null)
            where S : IDBusObject
        {
            if (connectionState != ConnectionState.Connected || currentConnection == null)
                throw new NotSupportedException("未连接");

            if (string.IsNullOrEmpty(name))
                name = path.ToServiceName();

            return currentConnection.CreateProxy<S>(name, path);
        }

        #endregion

        #region 注册新对象

        private readonly Dictionary<IDBusObject, string> registerDictionary = new Dictionary<IDBusObject, string>();

        public async Task RegisterNewObject(IDBusObject dbusObject, string targetName = null)
        {
            if (string.IsNullOrEmpty(targetName))
                targetName = dbusObject.ObjectPath.ToServiceName();

            lock (registerDictionary)
            {
                //添加物件与其目标名称添加到词典
                registerDictionary[dbusObject] = targetName;
            }

            if (connectionState == ConnectionState.Connected)
                await registerToConection(dbusObject).ConfigureAwait(false);
        }

        public async Task RegisterNewObjects(IDBusObject[] objects)
        {
            foreach (var dBusObject in objects)
                await RegisterNewObject(dBusObject).ConfigureAwait(false);
        }

        //bug: 注册的服务/物件在错误的dbus-send后会直接Name Lost，无法恢复
        private async Task registerObjects()
        {
            Logger.Log("注册DBus物件及服务...");

            //递归注册DBus服务
            foreach (var dBusObject in registerDictionary.Keys)
                await RegisterNewObject(dBusObject).ConfigureAwait(false);
        }

        private readonly List<string> registeredServices = new List<string>();

        private async Task registerToConection(IDBusObject dBusObject)
        {
            string targetName = registerDictionary[dBusObject];

            await currentConnection.RegisterObjectAsync(dBusObject).ConfigureAwait(false);
            await currentConnection.RegisterServiceAsync(targetName).ConfigureAwait(false);

            bool alreadyRegistered = false;

            lock (registeredServices)
            {
                alreadyRegistered = registeredServices.Contains(targetName);

                if (!alreadyRegistered)
                    registeredServices.Add(targetName);
            }

            if (!alreadyRegistered)
            {
                await currentConnection.ResolveServiceOwnerAsync(
                    targetName,
                    onServiceNameChanged,
                    e => onServiceError(e, dBusObject)).ConfigureAwait(false);
            }

            Logger.Log($"为{dBusObject.ObjectPath}注册{targetName}");
        }

        #endregion

        #region 反注册对象

        public void RemoveObject(IDBusObject dBusObject)
        {
            try
            {
                var target = registerDictionary.FirstOrDefault(o => o.Key.ObjectPath.Equals(dBusObject.ObjectPath)).Key;

                if (target != null)
                {
                    Logger.Log($"反注册{dBusObject.ObjectPath}");

                    string serviceName = registerDictionary[target];

                    lock (registerDictionary)
                        registerDictionary.Remove(target);

                    Task.Run(() => unRegisterFromConnection(target, serviceName).ConfigureAwait(false));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, $"移除DBus对象时遇到异常");
            }
        }

        private async Task unRegisterFromConnection(IDBusObject dBusObject, string serviceName)
        {
            currentConnection.UnregisterObject(dBusObject);
            await currentConnection.UnregisterServiceAsync(serviceName).ConfigureAwait(false);
        }

        #endregion

        #region 连接到DBus

        private CancellationTokenSource cancellationTokenSource;

        private string currentConnectTarget;

        public void Connect(string target = null)
        {
            if (isDisposed)
                throw new ObjectDisposedException(ToString(), "已处理的对象不能再次连接。");

            //默认连接到会话
            if (string.IsNullOrEmpty(Address.Session))
                throw new AddressNotFoundException("会话地址为空，请检查dbus服务是否已经启动");

            target ??= Address.Session;
            currentConnectTarget = target;

            //先停止服务
            //Disconnect();

            Logger.Log($"正在连接到 {target} 上的DBus服务!");

            //刷新cancellationToken
            cancellationTokenSource = new CancellationTokenSource();

            //开始服务
            Task.Run(() => connectTask(target), cancellationTokenSource.Token);
        }

        private async Task connectTask(string target)
        {
            try
            {
                switch (connectionState)
                {
                    case ConnectionState.NotConnected:
                        //初始化到DBus的连接
                        currentConnection ??= new Connection(target);

                        //连接到DBus
                        connectionState = ConnectionState.Connecting;

                        //等待连接
                        await currentConnection.ConnectAsync().ConfigureAwait(false);

                        //设置连接状态
                        connectionState = ConnectionState.Connected;

                        //注册对象
                        await registerObjects().ConfigureAwait(false);

                        OnConnected?.Invoke();
                        GreetService.SwitchState(true, "Initial connect");
                        break;

                    //case ConnectionState.Connected:
                    //    Logger.Log($"已经连接到{currentConnectTarget}，直接注册!");
                    //
                    //    //直接注册
                    //    await registerObjects().ConfigureAwait(false);
                    //    OnConnected?.Invoke();
                    //    GreetService.SwitchState(true, "");
                    //    break;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "连接到DBus时出现错误");
                connectionState = ConnectionState.Faulted;

                //Disconnect();
            }
        }

        #endregion

        private enum ConnectionState
        {
            NotConnected,
            Connecting,
            Connected,
            Faulted
        }

        private class AddressNotFoundException : InvalidOperationException
        {
            public AddressNotFoundException(string s)
                : base(s)
            {
            }
        }
    }
}
