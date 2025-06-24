#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using M.DBus.Utils;
using osu.Framework.Logging;
using Tmds.DBus;

namespace M.DBus
{
    [Obsolete]
    public class DBusManager<T> : IDisposable
        where T : IDBusObject
    {
        public Action OnConnected;

        private Connection currentConnection;

        public ConnectionState connectionState { get; private set; } = ConnectionState.NotConnected;

        private bool isDisposed { get; set; }

        public DBusManager()
        {
        }

        #region Disposal

        public void Dispose()
        {
            currentConnection.Dispose();

            isDisposed = true;
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

            Logger.Log($"为 {dBusObject.ObjectPath} 注册 {targetName}");
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

        public Task Connect(string target = null)
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
            return Task.Run(async () => await connectTask(target), cancellationTokenSource.Token);
        }

        private Task connectTask(string target)
        {
            if (connectionState == ConnectionState.Connected)
                return Task.FromException(new Exception("Already connected to DBus!"));

            try
            {
                //初始化到DBus的连接
                currentConnection ??= new Connection(target);

                //连接到DBus
                connectionState = ConnectionState.Connecting;

                //等待连接
                currentConnection.ConnectAsync().Wait();

                //设置连接状态
                connectionState = ConnectionState.Connected;

                OnConnected?.Invoke();

                Logger.Log("[Hikariii DBus] Connected to DBus!");

                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        #endregion

        public enum ConnectionState
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
