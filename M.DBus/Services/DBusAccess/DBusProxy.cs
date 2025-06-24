using Tmds.DBus.Protocol;
using Tmds.DBus.SourceGenerator;

namespace M.DBus.Services;

internal class DBusProxy : OrgFreedesktopDBusProxy
{
    public DBusProxy(Connection connection)
        : base(connection, "org.freedesktop.DBus", "/org/freedesktop/DBus")
    {
    }
}
