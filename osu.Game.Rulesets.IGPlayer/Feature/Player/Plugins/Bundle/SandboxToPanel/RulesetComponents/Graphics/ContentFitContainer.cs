using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace osu.Game.Rulesets.IGPlayer.Player.Plugins.Bundle.SandboxToPanel.RulesetComponents.Graphics
{
    public partial class ContentFitContainer : Container
    {
        protected override Container<Drawable> Content => content;

        private readonly Container content;

        public ContentFitContainer()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.Both;
            InternalChild = content = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                FillMode = FillMode.Fit,
            };
        }
    }
}
