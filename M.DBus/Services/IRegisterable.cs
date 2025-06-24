using Tmds.DBus.Protocol;

namespace M.DBus.Services;

public interface IRegisterable
{
    void register(Connection connection);
}
