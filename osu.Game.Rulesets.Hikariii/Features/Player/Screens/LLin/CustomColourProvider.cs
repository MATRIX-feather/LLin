using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Game.Overlays;
using osu.Game.Rulesets.Hikariii.Features.SystemIntegration.AccentColor;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Screens.LLin
{
    public partial class CustomColourProvider : OverlayColourProvider
    {
        public Color4 ActiveColor => AccentColor;
        public Color4 InActiveColor => Dark4;

        public Color4 AccentColor => accent ?? Highlight1;

        // 只是让代码变得更整齐一些...
        // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
        public Color4 ForegroundTextColor => textForeground;

        private Color4 textForeground = Color4.Black;

        private Color4? accent;

        public CustomColourProvider()
            : base(OverlayColourScheme.Pink)
        {
        }

        public CustomColourProvider(int hue)
            : base(hue)
        {
        }

        [Resolved]
        private AccentColorIntegration accentColor { get; set; } = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            UpdateHueColor(accentColor.GetAccentColor());
        }

        // 出于兼容性保留
        public BindableFloat HueColour = new();

        private static Color4 normalizeColor(Color4 c)
        {
            if (c.R <= 1f && c.G <= 1f && c.B <= 1f && c.A <= 1f)
                return c;

            return new Color4(
                c.R / 255f,
                c.G / 255f,
                c.B / 255f,
                c.A / 255f
            );
        }

        private float getLuminance(Color4 c)
        {
            float toLinear(float singleColor)
            {
                return singleColor <= 0.03928 ? singleColor / 12.92f : (float)Math.Pow((singleColor + 0.055) / 1.055, 2.4);
            }

            float red = toLinear(c.R);
            float green = toLinear(c.G);
            float blue = toLinear(c.B);

            return red * 0.2126f + green * 0.7152f + blue * 0.0722f;
        }

        public void UpdateHueColor(Color4 color)
        {
            color = normalizeColor(color);

            var hslColor = Color4.ToHsl(color);
            accent = color;

            //Logging.Log("New color is " + color);

            float colorLuminance = getLuminance(color);
            textForeground = colorLuminance > 0.179 ? Color4.Black : Color4.White;

            float hue = hslColor.X * 360f;

            ChangeColourScheme((int)hue);
            HueColour.Value = hue;
        }
    }
}
