using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Game.Overlays;
using osu.Game.Rulesets.Hikariii.Features.SystemIntegration.AccentColor;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Screens.LLin
{
    public partial class CustomColourProvider : OverlayColourProvider
    {
        public Color4 ActiveColor => Highlight1;
        public Color4 InActiveColor => Dark4;

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

        public void UpdateHueColor(Color4 color)
        {
            float hue = Color4.ToHsl(color).X * 360f;

            ChangeColourScheme((int)hue);
            HueColour.Value = hue;
        }
    }
}
