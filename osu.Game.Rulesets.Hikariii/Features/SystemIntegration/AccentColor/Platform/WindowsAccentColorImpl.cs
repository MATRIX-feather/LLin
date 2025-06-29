using System;
using osu.Framework.Graphics;
using osuTK.Graphics;

#if WINDOWS
using osu.Framework.Allocation;
using Windows.UI;
using Windows.UI.ViewManagement;
#endif

namespace osu.Game.Rulesets.Hikariii.Features.SystemIntegration.AccentColor.Platform;

public partial class WindowsAccentColorImpl : Drawable, IPlatformAccentColorImpl
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

    public event Action<Color4>? OnNewColorSet;

    public Color4 GetAccentColor()
    {
        return cachedAccentColor ?? Color4.White;
    }
}
