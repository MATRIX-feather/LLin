using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using NetCoreServer;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;
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
            if (Server != null)
            {
                Server.Stop();
                Server.Dispose();
                Server = null;
            }

            startServer();
        }

        public void Boardcast(string text)
        {
            if (Server == null) throw new NullDependencyException("Server not initialized");

            Server.MulticastText(text);
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

                Logging.Log("Done!");
                Logging.Log($"WS Server opened at http://{Server.Address}:{Server.Port}");
            }
            catch (Exception e)
            {
                Logging.Log($"无法启动WebSocket服务器: {e}", level: LogLevel.Important);
                Logging.Log(e.ToString());
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            Server?.Dispose();

            base.Dispose(isDisposing);
        }

        public GosuServer? Server;

        public partial class GosuServer : WsServer
        {
            public GosuServer(IPAddress address, int port)
                : base(address, port)
            {
            }

            protected override TcpSession CreateSession() { return new GosuSession(this); }

            protected override void OnError(SocketError error)
            {
                Logging.Log($"Chat WebSocket server caught an error with code {error}");
            }
        }

        private partial class GosuSession : WsSession
        {
            public GosuSession(NetCoreServer.WsServer server)
                : base(server)
            {
            }

            public override void OnWsConnected(HttpRequest request)
            {
                Logging.Log($"Chat WebSocket session with Id {Id} connected!");
            }

            public override void OnWsDisconnected()
            {
                Logging.Log($"Chat WebSocket session with Id {Id} disconnected!");
            }

            public override void OnWsReceived(byte[] buffer, long offset, long size)
            {
                string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
                Logging.Log("WebSocket Incoming: " + message);
            }

            public override void OnWsError(string error)
            {
                Logging.Log("WS ERRORED: " + error);
            }

            public override void OnWsError(SocketError error)
            {
                Logging.Log("WS ERRORED: " + error);
            }

            protected override void OnError(SocketError error)
            {
                Logging.Log($"Chat WebSocket session caught an error with code {error}");
            }
        }
    }
}
