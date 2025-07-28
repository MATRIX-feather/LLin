using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.BottomBar.Buttons
{
    public partial class BottomBarSwitchButton : BottomBarButton
    {
        public BindableBool Value = new BindableBool();

        public bool Default { get; set; }

        protected Color4 ActivateColor => ColourProvider.AccentColor;
        protected Color4 InActivateColor => ColourProvider.Background3; // ActivateColor.Darken(0.75f);
        protected Color4 ActivateColorBorder => ActivateColor.Lighten(0.25f);
        protected Colour4 InActivateColorBorder => InActivateColor.Darken(0.25f);

        public BottomBarSwitchButton(IToggleableFunctionProvider provider)
            : base(provider)
        {
            Value.Value = Default;

            Value.BindTo(provider.Bindable);
        }

        protected override void LoadComplete()
        {
            Value.BindValueChanged(_ => updateVisuals(true), true);
            Value.BindDisabledChanged(onDisabledChanged, true);

            ColourProvider.HueColour.BindValueChanged(_ => updateVisuals());

            base.LoadComplete();
        }

        private void onDisabledChanged(bool disabled)
        {
            this.FadeColour(disabled ? Color4.Gray : Color4.White, 300, Easing.OutQuint);
        }

        protected override bool OnClick(ClickEvent e)
        {
            if (!Value.Disabled) return base.OnClick(e);

            this.FlashColour(Color4.Red, 1000, Easing.OutQuint);
            return false;
        }

        private void updateVisuals(bool animate = false)
        {
            int duration = animate ? 500 : 0;

            switch (Value.Value)
            {
                case true:
                    BgBox.FadeColour(ActivateColor, duration, Easing.OutQuint);
                    ContentFillFlow.FadeColour(ColourProvider.ForegroundTextColor, duration, Easing.OutQuint);
                    outerContent!.BorderColour = ColourInfo.GradientVertical(ActivateColor, ActivateColorBorder);

                    if (animate)
                        OnToggledOnAnimation();
                    break;

                case false:
                    BgBox.FadeColour(InActivateColor, duration, Easing.OutQuint);
                    ContentFillFlow.FadeColour(Colour4.White, duration, Easing.OutQuint);
                    outerContent!.BorderColour = ColourInfo.GradientVertical(InActivateColor, InActivateColorBorder);
                    break;
            }
        }

        protected virtual void OnToggledOnAnimation()
        {
        }
    }
}
