using System;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;
using Tmds.DBus.SourceGenerator;

namespace LLin.OSIntegrations.Linux.DBus.Services;

public class FreedesktopSettingsAccessor
{
    internal OrgFreedesktopPortalSettingsProxy proxy;

    public FreedesktopSettingsAccessor(Connection bindingConnection)
    {
        this.proxy = new(bindingConnection, "org.freedesktop.portal.Desktop", "/org/freedesktop/portal/desktop");

        proxy.WatchSettingChangedAsync(triggerSettingsChanged);
    }

    private void triggerSettingsChanged(Exception? arg1, (string Namespace, string Key, VariantValue Value) arg2)
    {
        OnSettingsChanged?.Invoke(arg1, arg2);
    }

    public event Action<Exception?, (string optionNamespace, string optionName, VariantValue value)>? OnSettingsChanged;

    public Task<VariantValue> ReadSettingAsync(string optionNamespace, string optionName)
    {
        return proxy.ReadAsync(optionNamespace, optionName);
    }

    /// <exception cref="DBusException"></exception>
    public VariantValue ReadSetting(string optionNamespace, string optionName)
    {
        return ReadSettingAsync(optionNamespace, optionName)
               .GetAwaiter()
               .GetResult();
    }
}
