#nullable disable

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Screens.LLin;
using osu.Game.Rulesets.IGPlayer.Helper.Configuration;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Misc
{
    public partial class BgTrianglesContainer : VisibilityContainer
    {
        private const float triangles_alpha = 0.65f;
        private readonly Bindable<bool> enableBgTriangles = new Bindable<bool>();
        private Container trianglesContainer;

        [BackgroundDependencyLoader]
        private void load(MConfigManager config)
        {
            RelativeSizeAxes = Axes.Both;
            State.Value = Visibility.Visible;

            Child = trianglesContainer = new MBgTriangles(triangleScale: 4f, sync: true)
            {
                Alpha = triangles_alpha,
                RelativeSizeAxes = Axes.Both
            };
            config.BindWith(MSetting.MvisEnableBgTriangles, enableBgTriangles);
        }

        protected override void LoadComplete()
        {
            enableBgTriangles.BindValueChanged(updateVisuals, true);

            base.LoadComplete();
        }

        private void updateVisuals(ValueChangedEvent<bool> value)
        {
            switch (value.NewValue)
            {
                case true:
                    trianglesContainer.FadeTo(triangles_alpha, 250);
                    break;

                case false:
                    trianglesContainer.FadeOut(250);
                    break;
            }
        }

        protected override void PopIn()
        {
            this.FadeIn(250);
        }

        protected override void PopOut()
        {
            this.FadeOut(250);
        }
    }
}
