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

namespace osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Theme.Platform;

public partial class LinuxThemeImpl : Drawable, IPlatformThemeImpl
{
    [Resolved]
    private DBusIntegration dbusIntegration { get; set; } = null!;

    private Color4? cachedLastValidColor;
    private ColorScheme? cachedLastValidColorScheme;
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
            applyAccentColorFromVariant(accentColorSetting);

            var colorScheme = await settingsAccessor.ReadSettingAsync("org.freedesktop.appearance", "color-scheme");
            applyColorSchemeFromVariant(colorScheme);
        }
        catch (DBusException e)
        {
            Logging.LogError(e, "Failed reading system theme from desktop portal");
        }
        catch (ParseException e)
        {
            Logging.LogError(e, "Failed parsing system theme from desktop portal");
        }
        catch (Exception e)
        {
            Logging.LogError(e, "Unknown error occurred while processing system theme integration");
        }
    }

    public class ParseException(string message) : Exception(message);

    private void onDesktopSettingsChanged(Exception? arg1,
                                          (string optionNamespace, string optionName, VariantValue value) pair)
    {
        if (pair.optionNamespace != "org.freedesktop.appearance")
            return;

        switch (pair.optionName)
        {
            case "accent-color":
                applyAccentColorFromVariant(pair.value);
                break;

            case "color-scheme":
                applyColorSchemeFromVariant(pair.value);
                break;
        }
    }

    /// <exception cref="Exception">Parse error</exception>
    private void applyColorSchemeFromVariant(VariantValue variant)
    {
        var structUnpack = variant.Type == VariantValueType.Variant
            ? variant.GetVariantValue()
            : variant;

        if (structUnpack.Type != VariantValueType.UInt32)
            throw new ParseException($"Bad desktop implementation? Expected UInt32 but got {structUnpack.Type}");

        uint scheme = structUnpack.GetUInt32();
        if (scheme is < 0 or > 2) scheme = 0;

        ColorScheme schemeEnum = scheme switch
        {
            0 => ColorScheme.NONE,
            1 => ColorScheme.PREFER_DARK,
            2 => ColorScheme.PREFER_LIGHT,
            _ => throw new ParseException($"Invalid color scheme {scheme}")
        };

        //Logging.Log("AAAAAA COLOR SCHEME IS " + schemeEnum);

        var lastValue = cachedLastValidColorScheme ?? null;
        cachedLastValidColorScheme = schemeEnum;

        if (lastValue != schemeEnum)
            OnColorSchemeSet?.Invoke(schemeEnum);
    }

    /// <exception cref="Exception">Parse error</exception>
    private void applyAccentColorFromVariant(VariantValue variant)
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
                throw new ParseException($"Variant {item} is not double");

            return (float)item.GetDouble();
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        session?.Dispose();
    }

    public event Action<Color4>? OnNewColorSet;
    public event Action<ColorScheme>? OnColorSchemeSet;

    public Color4 GetAccentColor()
    {
        return cachedLastValidColor ?? Color4.White;
    }

    public ColorScheme GetColorScheme()
    {
        return cachedLastValidColorScheme ?? ColorScheme.NONE;
    }
}
