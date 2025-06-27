using System;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hikariii.Features.SystemIntegration.AccentColor;

public interface IPlatformAccentColorImpl
{
    event Action<Color4> OnNewColorSet;

    Color4 GetAccentColor();
}
