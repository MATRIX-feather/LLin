using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Notifications.Platform;

namespace osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Notifications;

public partial class SystemNotificationIntegration : CompositeDrawable
{
    private INotificationImpl? integrationImpl;

    [BackgroundDependencyLoader]
    private void load()
    {
        var impl = tryImpl();

        integrationImpl = impl;
        if (impl is Drawable drawable)
            AddInternal(drawable);
    }

    private INotificationImpl? tryImpl()
    {
        if (OperatingSystem.IsLinux()) return new LinuxNotificationImpl();

        Logging.Log("Current platform doesn't support notification integration");
        return null;
    }

    public void SetDoNotDisturb(bool doNotDisturb, string reason)
    {
        if (doNotDisturb)
            integrationImpl?.EnableDoNotDisturb(reason);
        else
            integrationImpl?.DisableDoNotDisturb();
    }
}
