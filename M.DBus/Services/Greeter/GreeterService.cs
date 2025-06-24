using Tmds.DBus.Protocol;

namespace M.DBus.Services;

public class GreeterService : IRegisterable
{
    private Greet greet;

    public void register(Connection connection)
    {
        greet ??= new Greet(connection);
        connection.AddMethodHandler(greet);
    }
}
