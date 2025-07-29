using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Hikariii.Features.Configuration;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Theme.Platform;

public partial class MfosuThemeImpl : Drawable, IPlatformThemeImpl
{
    private readonly BindableFloat colorRed = new();
    private readonly BindableFloat colorGreen = new();
    private readonly BindableFloat colorBlue = new();

    public event Action<Color4>? OnNewColorSet;
    public event Action<ColorScheme>? OnColorSchemeSet;

    public Color4 GetAccentColor()
    {
        return new Color4(colorRed.Value, colorGreen.Value, colorBlue.Value, 255f);
    }

    public ColorScheme GetColorScheme()
    {
        return ColorScheme.PREFER_DARK;
    }

    [BackgroundDependencyLoader]
    private void load(MConfigManager config)
    {
        config.BindWith(MSetting.MvisInterfaceRed, colorRed);
        config.BindWith(MSetting.MvisInterfaceGreen, colorGreen);
        config.BindWith(MSetting.MvisInterfaceBlue, colorBlue);

        colorRed.BindValueChanged(_ => updateColor());
        colorGreen.BindValueChanged(_ => updateColor());
        colorBlue.BindValueChanged(_ => updateColor());
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        updateColor();
    }

    private void updateColor()
    {
        var hslColor = Color4.ToHsl(GetAccentColor());
        var finalColor = Color4.FromHsl(new Vector4(hslColor.X, 1f, 0.7f, 1f));

        OnNewColorSet?.Invoke(finalColor);
    }
}
