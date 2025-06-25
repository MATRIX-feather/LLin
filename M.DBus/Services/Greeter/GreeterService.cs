using Tmds.DBus.Protocol;

namespace M.DBus.Services;

public class GreeterService(string host)
{
    private Greet? greet;

    public void Register(Connection connection)
    {
        greet ??= new Greet(connection, host);
        connection.AddMethodHandler(greet.PathHandler!);
    }
}
