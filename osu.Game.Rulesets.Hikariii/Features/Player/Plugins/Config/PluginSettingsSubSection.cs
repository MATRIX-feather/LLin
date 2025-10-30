using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Overlays.Settings;
using osuTK;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config
{
    public partial class PluginSettingsSubsection : SettingsSection
    {
        private readonly LLinPluginProvider provider;

        public PluginSettingsSubsection(LLinPluginProvider plugin)
        {
            this.provider = plugin;
            Name = $"{plugin}的subsection";

            AutoSizeAxes = Axes.Y;
            RelativeSizeAxes = Axes.X;
        }

        public override Drawable CreateIcon()
        {
            return new SpriteIcon
            {
                Icon = FontAwesome.Solid.Atom,
                Size = new Vector2(18)
            };
        }

        public override LocalisableString Header => provider.GetDescription().Name;

        [BackgroundDependencyLoader]
        private void load(LLinPluginManager pluginManager)
        {
            var entries = pluginManager.GetSettingsFor(provider);

            foreach (var se in entries)
                Add(se.ToSettingsItem());
        }
    }
}
