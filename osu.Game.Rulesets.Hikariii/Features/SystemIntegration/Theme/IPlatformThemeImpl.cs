using System;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Theme;

public interface IPlatformThemeImpl
{
    event Action<Color4> OnNewColorSet;

    event Action<ColorScheme> OnColorSchemeSet;

    Color4 GetAccentColor();
    ColorScheme GetColorScheme();
}
