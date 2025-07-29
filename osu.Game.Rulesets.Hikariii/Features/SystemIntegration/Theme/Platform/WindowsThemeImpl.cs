using System;
#if WINDOWS
using Windows.UI;
using Windows.UI.ViewManagement;
using osu.Framework.Allocation;
#endif
using osu.Framework.Graphics;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Theme.Platform;

public partial class WindowsThemeImpl : Drawable, IPlatformThemeImpl
{
#if WINDOWS

    private UISettings? windowsUISettings;

    [BackgroundDependencyLoader]
    private void load()
    {
        windowsUISettings = new UISettings();

        var windowsAccentColor = windowsUISettings.GetColorValue(UIColorType.Accent);
        cachedAccentColor = convertColor(windowsAccentColor);

        windowsUISettings.ColorValuesChanged += onWindowsColorChanged;
    }

    private void onWindowsColorChanged(UISettings uiSettings, object args)
    {
        var convertedColor = convertColor(uiSettings.GetColorValue(UIColorType.Accent));
        this.cachedAccentColor = convertedColor;
        OnNewColorSet?.Invoke(convertedColor);
    }

    private Color4 convertColor(Color windowsColor)
    {
        return new Color4(windowsColor.R, windowsColor.G, windowsColor.B, windowsColor.A);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        if (windowsUISettings != null)
            windowsUISettings.ColorValuesChanged += onWindowsColorChanged;
    }
#endif

    public static bool Available()
    {
#if WINDOWS
        return true;
#else
        return false;
#endif
    }

    private Color4? cachedAccentColor;
    private ColorScheme? cachedAccentColorScheme;

    public event Action<Color4>? OnNewColorSet;
    public event Action<ColorScheme>? OnColorSchemeSet;

    public Color4 GetAccentColor()
    {
        return cachedAccentColor ?? Color4.White;
    }

    public ColorScheme GetColorScheme()
    {
        // todo: Implement this
        return cachedAccentColorScheme ?? ColorScheme.NONE;
    }
}
