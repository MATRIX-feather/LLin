using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Hikariii.Features.Configuration;
using osu.Game.Rulesets.Hikariii.Features.Player.Screens.LLin;
using osu.Game.Rulesets.Hikariii.Features.SystemIntegration.AccentColor.Platform;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hikariii.Features.SystemIntegration.AccentColor;

public partial class AccentColorIntegration : CompositeDrawable
{
    private IPlatformAccentColorImpl? platformAccentColorImpl;

    protected IPlatformAccentColorImpl? PlatformAccentColorImpl
    {
        get => platformAccentColorImpl;
        set
        {
            defaultAccentColorImpl.OnNewColorSet -= OnPlatformAccentColorChanged;

            var last = platformAccentColorImpl;
            platformAccentColorImpl = value;

            if (last != null)
                last.OnNewColorSet -= OnPlatformAccentColorChanged;

            if (value != null)
            {
                value.OnNewColorSet += OnPlatformAccentColorChanged;
                Schedule(() => OnPlatformAccentColorChanged(value.GetAccentColor()));
            }
            else
            {
                defaultAccentColorImpl.OnNewColorSet += OnPlatformAccentColorChanged;
                Schedule(() => OnPlatformAccentColorChanged(defaultAccentColorImpl.GetAccentColor()));
            }
        }
    }

    private void OnPlatformAccentColorChanged(Color4 newColor)
    {
        Schedule(() =>
        {
            customColors.UpdateHueColor(newColor);
            OnNewColorSet?.Invoke(newColor);
        });
    }

    private readonly MfosuAccentColorImpl defaultAccentColorImpl = new();

    [Resolved]
    private CustomColourProvider customColors { get; set; } = null!;

    [Resolved]
    private MConfigManager config { get; set; } = null!;

    private readonly BindableBool usePlatformAccentColor = new();

    [BackgroundDependencyLoader]
    private void load()
    {
        config.BindWith(MSetting.UsePlatformAccentColor, usePlatformAccentColor);
        AddInternal(defaultAccentColorImpl);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        usePlatformAccentColor.BindValueChanged(v =>
        {
            if (v.NewValue)
            {
                var newImpl = selectImplementation();
                PlatformAccentColorImpl = newImpl;
            }
            else
            {
                if (platformAccentColorImpl is Drawable d
                    && platformAccentColorImpl != defaultAccentColorImpl
                    && d.Parent == this)
                {
                    RemoveInternal(d, true);
                }

                PlatformAccentColorImpl = defaultAccentColorImpl;
            }
        }, true);
    }

    private IPlatformAccentColorImpl? selectImplementation()
    {
        if (OperatingSystem.IsLinux())
        {
            var impl = new LinuxAccentColorImpl();
            LoadComponent(impl);
            AddInternal(impl);

            return impl;
        }

        if (OperatingSystem.IsWindows())
        {
            var impl = new WindowsAccentColorImpl();
            LoadComponent(impl);
            AddInternal(impl);

            return impl;
        }

        Logging.Log("Platform accent colorizer not supported");
        return null;
    }

    public event Action<Color4>? OnNewColorSet;

    public Color4 GetAccentColor()
    {
        return platformAccentColorImpl?.GetAccentColor() ?? defaultAccentColorImpl.GetAccentColor();
    }
}
