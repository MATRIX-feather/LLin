using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using LLin.OSIntegrations.Linux.DBus;
using LLin.OSIntegrations.Linux.DBus.Services;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using osu.Game.Rulesets.Hikariii.Features.SystemIntegration.DBus;

namespace osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Notifications.Platform;

public partial class LinuxNotificationImpl : Drawable, INotificationImpl
{
    [Resolved]
    private DBusIntegration dbusIntegration { get; set; } = null!;

    private FreedesktopNotificationsAccessor? accessor;
    private DBusSession? session;

    [BackgroundDependencyLoader]
    private void load()
    {
        session = dbusIntegration.AcquireNewSession();
        session.OnConnected += onConnected;
        Task.Run(async () => await session.Connect());
    }

    private void onConnected(DBusSession session)
    {
        Debug.Assert(session.CurrentConnection != null);
        accessor = new FreedesktopNotificationsAccessor(session.CurrentConnection);
    }

    private uint? doNotDisturbHandle;
    private CancellationTokenSource? cancellationTokenSource;

    private volatile bool inhibitSupported = true;

    public void EnableDoNotDisturb(string reason)
    {
        if (accessor == null || !inhibitSupported) return;

        cancellationTokenSource?.Cancel();
        cancellationTokenSource = new CancellationTokenSource();

        Task.Run(async () =>
        {
            try
            {
                uint handle = await accessor.InhibitAsync("osu!", reason, []);
                doNotDisturbHandle = handle;
                Logging.Log($"DONE Invoking Inhibit async! {handle}");
            }
            catch (Exception e)
            {
                inhibitSupported = false;
                Logging.LogError(e, "Calling inhibit failed, ignoring further calls...", LogLevel.Verbose);
            }
        }, cancellationTokenSource.Token);
    }

    public void DisableDoNotDisturb()
    {
        cancellationTokenSource?.Cancel();

        uint? handle = doNotDisturbHandle;
        doNotDisturbHandle = null;

        if (handle != null)
            accessor?.UnInhibitAsync((uint)handle);
    }
}
