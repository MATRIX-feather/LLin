using System;
using System.Diagnostics;
using System.Threading.Tasks;
using LLin.OSIntegrations.Linux.DBus;
using LLin.OSIntegrations.Linux.DBus.Services;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Hikariii.Features.SystemIntegration.DBus;
using osuTK.Graphics;
using Tmds.DBus.Protocol;

namespace osu.Game.Rulesets.Hikariii.Features.SystemIntegration.AccentColor.Platform;

public partial class LinuxAccentColorImpl : Drawable, IPlatformAccentColorImpl
{
    [Resolved]
    private DBusIntegration dbusIntegration { get; set; } = null!;

    private Color4? cachedLastValidColor;
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
        Task.Run(async () => await runInitialSetup(session));
    }

    private async Task runInitialSetup(DBusSession session)
    {
        Debug.Assert(session.CurrentConnection != null);

        var settingsAccessor = new FreedesktopSettingsAccessor(session.CurrentConnection);
        settingsAccessor.OnSettingsChanged += onDesktopSettingsChanged;

        try
        {
            var accentColorSetting = await settingsAccessor.ReadSettingAsync("org.freedesktop.appearance", "accent-color");
            readAccentColorFromVariant(accentColorSetting);
        }
        catch (DBusException e)
        {
            Logging.LogError(e, "Failed reading accent color from desktop portal");
        }
    }

    private void onDesktopSettingsChanged(Exception? arg1,
                                          (string optionNamespace, string optionName, VariantValue value) pair)
    {
        if (pair.optionNamespace != "org.freedesktop.appearance")
            return;

        if (pair.optionName != "accent-color")
            return;

        readAccentColorFromVariant(pair.value);
    }

    /// <exception cref="Exception">Parse error</exception>
    private void readAccentColorFromVariant(VariantValue variant)
    {
        float red, green, blue;

        try
        {
            var structUnpack = variant.Type == VariantValueType.Variant
                ? variant.GetVariantValue()
                : variant;

            red = readColorSingle(structUnpack.GetItem(0));
            green = readColorSingle(structUnpack.GetItem(1));
            blue = readColorSingle(structUnpack.GetItem(2));
        }
        catch (Exception e)
        {
            Logging.LogError(e, "Failed to read accent color from desktop portal");
            return;
        }

        var newColor = new Color4(red, green, blue, 1);
        cachedLastValidColor = newColor;
        OnNewColorSet?.Invoke(newColor);
        return;

        float readColorSingle(VariantValue item)
        {
            if (item.Type != VariantValueType.Double)
                throw new Exception($"Variant {item} is not double");

            return (float)item.GetDouble();
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        session?.Dispose();
    }

    public event Action<Color4>? OnNewColorSet;

    public Color4 GetAccentColor()
    {
        return cachedLastValidColor ?? Color4.White;
    }
}
