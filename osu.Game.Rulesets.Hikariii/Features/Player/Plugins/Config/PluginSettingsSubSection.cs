using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Extensions;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins;
using osuTK;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config
{
    public partial class PluginSettingsSubsection : SettingsSection
    {
        private readonly IHikariiiPluginProvider provider;

        public PluginSettingsSubsection(IHikariiiPluginProvider plugin)
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

        public override LocalisableString Header => provider.GetPluginDescription().Name;

        [BackgroundDependencyLoader]
        private void load(IHikariiiPluginManager pluginManager)
        {
            var entries = provider.GetSettingsEntries(pluginManager.TryGetPluginConfigOrThrow<IPluginConfigManager>(provider));

            foreach (var se in entries)
                Add(se.ToSettingsItem());
        }
    }
}
