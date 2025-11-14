using System.Collections.Generic;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;
using Tmds.DBus.SourceGenerator;

namespace LLin.OSIntegrations.Linux.DBus.Services;

public class FreedesktopNotificationsAccessor
{
    internal OrgFreedesktopNotificationsProxy proxy;

    public FreedesktopNotificationsAccessor(Connection bindingConnection)
    {
        this.proxy = new(bindingConnection, "org.freedesktop.Notifications", "/org/freedesktop/Notifications");
    }

    public Task<uint> InhibitAsync(string desktopEntry, string reason, Dictionary<string, VariantValue> hints)
    {
        return proxy.InhibitAsync(desktopEntry, reason, hints);
    }

    public Task UnInhibitAsync(uint handleID)
    {
        return proxy.UnInhibitAsync(handleID);
    }
}
