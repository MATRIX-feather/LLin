using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using NetCoreServer;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Data;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Web
{
    public partial class WebSocketLoader : CompositeDrawable
    {
        public readonly DataRoot DataRoot = new DataRoot();

        public WebSocketLoader()
        {
            AlwaysPresent = true;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Logging.Log("WS LOAD!");
            Schedule(startServer);
        }

        public void Restart()
        {
            stopServer();
            startServer();
        }

        public void Boardcast(string text)
        {
            if (Server == null) throw new NullDependencyException("Server not initialized");

            Server.MulticastText(text);
        }

        private void stopServer()
        {
            if (Server == null) return;

            Server.Stop();
            Server.Dispose();

            try
            {
                OnServerStop?.Invoke(Server);
            }
            catch (Exception e)
            {
                Logging.Log($"Error occurred calling OnServerStop: {e.Message}");
                Logging.Log(e.StackTrace ?? "<No stacktrace>");
            }

            Server = null;
        }

        private void startServer()
        {
            Logging.Log("Initializing WebSocket Server...");

            try
            {
                var ip = IPAddress.Loopback;
                int port = 24050;

                this.Server = new GosuServer(ip, port);

                Server.Start();

                try
                {
                    OnServerStart?.Invoke(Server);
                }
                catch (Exception e)
                {
                    Logging.Log($"Error occurred calling OnServerStart: {e.Message}");
                    Logging.Log(e.StackTrace ?? "<No stacktrace>");
                }

                Logging.Log("Done!");
                Logging.Log($"WS Server opened at http://{Server.Address}:{Server.Port}");
            }
            catch (Exception e)
            {
                Logging.Log($"无法启动WebSocket服务器: {e}", level: LogLevel.Important);
                Logging.Log(e.ToString());
            }
        }

        public Action<GosuServer>? OnServerStart;
        public Action<GosuServer>? OnServerStop;

        protected override void Dispose(bool isDisposing)
        {
            stopServer();

            base.Dispose(isDisposing);
        }

        public GosuServer? Server;

        public partial class GosuServer : WsServer
        {
            public GosuServer(IPAddress address, int port)
                : base(address, port)
            {
            }

            private Storage? storage;

            public void SetStorage(Storage storage)
            {
                this.storage = storage;
            }

            public Storage? GetStorage()
            {
                return this.storage;
            }

            protected override TcpSession CreateSession() { return new GosuSession(this); }

            protected override void OnError(SocketError error)
            {
                Logging.Log($"Chat WebSocket server caught an error with code {error}");
            }

            public void AddCustomHandler(string path, string urlPath, FileCache.InsertHandler handler)
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(100);
                this.Cache.InsertPath(path, urlPath, "*.*", timeout, handler);
            }

            /**
             * From decompiled HttpServer#AddStaticContent(...)
             */
            public new void AddStaticContent(string path, string prefix = "/", string filter = "*.*", TimeSpan? timeout = null)
            {
                timeout ??= TimeSpan.FromHours(1.0);

                this.Cache.InsertPath(path, prefix, filter, timeout.Value, handler);

                static bool handler(FileCache cache, string key, byte[] value, TimeSpan timespan)
                {
                    var response = new HttpResponse();
                    response.SetBegin(200);
                    response.SetContentType(Path.GetExtension(key));

                    var interpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 1);
                    interpolatedStringHandler.AppendLiteral("max-age=");
                    interpolatedStringHandler.AppendFormatted(timespan.Seconds);

                    string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                    response.SetHeader("Cache-Control", stringAndClear);
                    response.SetHeader("Access-Control-Allow-Origin", "*");
                    response.SetBody(value);

                    return cache.Add(key, response.Cache.Data, timespan);
                }
            }
        }
    }
}
