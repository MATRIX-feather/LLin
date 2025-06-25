using System.Threading.Tasks;
using Tmds.DBus.Protocol;
using Tmds.DBus.SourceGenerator;

namespace M.DBus.Services
{
    internal class Greet : IoMatrixFeatherMfosuGreeterHandler
    {
        private readonly string host;

        public Greet(Connection connection, string host = "unknown")
        {
            this.host = host;
            Connection = connection;

            PathHandler = new PathHandler(this.Path);
            PathHandler.Add(this);
        }

        public void SwitchState(bool online, string reason)
        {
            if (!online)
                EmitGoingOffline(reason);
        }

        public override Connection Connection { get; }

        protected override ValueTask<string> OnGreetAsync(Message request)
        {
            return new ValueTask<string>(host);
        }

        public ValueTask HandleMethodAsync(MethodContext context)
        {
            return PathHandler!.HandleMethodAsync(context);
        }

        public bool RunMethodHandlerSynchronously(Message message)
        {
            return PathHandler!.RunMethodHandlerSynchronously(message);
        }

        public string Path { get; } = "/io/matrix_feather/mfosu/greeter";
    }
}
