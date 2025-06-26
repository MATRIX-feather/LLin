using System.Threading.Tasks;

namespace LLin.OSIntegrations.Linux.DBus.Services.DBusAccess;

public class DBusAccess
{
    internal DBusProxy Proxy { get; set; }

    internal DBusAccess(DBusProxy proxy)
    {
        this.Proxy = proxy;
    }

    public Task<uint> RequestName(string serviceName, uint flags)
    {
        return Proxy.RequestNameAsync(serviceName, flags);
    }
}
