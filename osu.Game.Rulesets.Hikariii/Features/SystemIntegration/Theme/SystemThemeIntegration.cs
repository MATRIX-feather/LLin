using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Hikariii.Features.Configuration;
using osu.Game.Rulesets.Hikariii.Features.Player.Screens.LLin;
using osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Theme.Platform;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Theme;

public partial class SystemThemeIntegration : CompositeDrawable
{
    private IPlatformThemeImpl? platformAccentColorImpl;

    protected IPlatformThemeImpl? PlatformAccentColorImpl
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

    private readonly MfosuThemeImpl defaultAccentColorImpl = new();

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
                if (newImpl == null)
                    Logging.Log("Platform accent colorizer not supported");

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

    private IPlatformThemeImpl? selectImplementation()
    {
        if (OperatingSystem.IsLinux())
        {
            var impl = new LinuxThemeImpl();
            LoadComponent(impl);
            AddInternal(impl);

            return impl;
        }

        if (OperatingSystem.IsWindows())
        {
            if (!WindowsThemeImpl.Available())
            {
                Logging.Log("Detected Windows, but this version of LLin has not been built with Windows integration!");
                return null;
            }

            var impl = new WindowsThemeImpl();
            LoadComponent(impl);
            AddInternal(impl);

            return impl;
        }

        return null;
    }

    public event Action<Color4>? OnNewColorSet;

    public Color4 GetAccentColor()
    {
        return platformAccentColorImpl?.GetAccentColor() ?? defaultAccentColorImpl.GetAccentColor();
    }
}
