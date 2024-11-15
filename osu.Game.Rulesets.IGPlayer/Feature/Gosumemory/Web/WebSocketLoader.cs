using System;
using System.Net;
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
            stopServer();
            startServer();
        }

        public void Broadcast(string text)
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
    }
}
