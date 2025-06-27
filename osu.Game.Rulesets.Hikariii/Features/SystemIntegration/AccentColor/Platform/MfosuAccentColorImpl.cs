using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Hikariii.Features.Configuration;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hikariii.Features.SystemIntegration.AccentColor.Platform;

public partial class MfosuAccentColorImpl : Drawable, IPlatformAccentColorImpl
{
    private readonly BindableFloat colorRed = new();
    private readonly BindableFloat colorGreen = new();
    private readonly BindableFloat colorBlue = new();

    public event Action<Color4>? OnNewColorSet;

    public Color4 GetAccentColor()
    {
        return new Color4(colorRed.Value, colorGreen.Value, colorBlue.Value, 255f);
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
        OnNewColorSet?.Invoke(GetAccentColor());
    }
}
