#nullable disable

using System;
using System.Numerics;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics.UserInterface;
using Vector2 = osuTK.Vector2;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SideBar.Settings.Items
{
    public partial class SettingsSlider<T> : OsuSliderBar<T>
        where T : struct, INumber<T>, IMinMaxValue<T>, IConvertible
    {
        /// <summary>
        /// Controls whether to show a hint line below the slider body.
        /// </summary>
        public readonly BindableBool HintActive = new BindableBool();

        private Circle baseLine;
        private Container circle;

        [BackgroundDependencyLoader]
        private void load()
        {
            Height = 1;

            Children = new Drawable[]
            {
                baseLine = new Circle
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.X,
                    Height = 2,
                    Alpha = 0,
                },
                circle = new Container
                {
                    RelativePositionAxes = Axes.X,
                    Size = new Vector2(25),
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Child = new Circle
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        RelativePositionAxes = Axes.X,
                        X = -0.5f
                    }
                }
            };

            RangePadding = 0;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            HintActive.BindValueChanged(onLineStatusChanged);
        }

        private void onLineStatusChanged(ValueChangedEvent<bool> e)
        {
            if (e.NewValue)
            {
                baseLine.ResizeHeightTo(4, 250, Easing.OutExpo);
                baseLine.FadeTo(0.8f, 250, Easing.OutExpo);
            }
            else
            {
                baseLine.ResizeHeightTo(2, 250, Easing.OutExpo);
                baseLine.FadeOut(250, Easing.OutExpo);
            }
        }

        protected override void UpdateValue(float value)
        {
            circle.MoveToX(value, 250, Easing.OutExpo);
            circle.ScaleTo(value + 0.2f, 250, Easing.OutBack);
        }
    }
}
