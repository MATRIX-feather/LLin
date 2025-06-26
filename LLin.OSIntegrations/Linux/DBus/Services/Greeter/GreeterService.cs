using Tmds.DBus.Protocol;

namespace LLin.OSIntegrations.Linux.DBus.Services.Greeter;

public class GreeterService(string host)
{
    private Greet? greet;

    public void Register(Connection connection)
    {
        greet ??= new Greet(connection, host);
        connection.AddMethodHandler(greet.PathHandler!);
    }
}
