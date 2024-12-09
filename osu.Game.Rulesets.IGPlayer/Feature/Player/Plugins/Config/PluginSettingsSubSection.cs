using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Interfaces.Plugins;
using osuTK;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Config
{
    [Obsolete("请使用GetSettingEntries")]
    public abstract partial class PluginSettingsSubSection : SettingsSubsection
    {
        private readonly LLinPlugin plugin;
        protected IPluginConfigManager ConfigManager = null!;

        protected override LocalisableString Header => plugin.Name;

        protected PluginSettingsSubSection(LLinPlugin plugin)
        {
            this.plugin = plugin;
        }

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            var dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
            ConfigManager = dependencies.Get<LLinPluginManager>().GetConfigManager(plugin);
            return dependencies;
        }
    }

    public partial class PluginSettingsSubsection : SettingsSection
    {
        private readonly LLinPlugin plugin;

        public PluginSettingsSubsection(LLinPlugin plugin)
        {
            this.plugin = plugin;
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

        public override LocalisableString Header => plugin.Name;

        [BackgroundDependencyLoader]
        private void load(LLinPluginManager pluginManager)
        {
            var entries = pluginManager.GetSettingsFor(plugin);

            if (entries == null) return;

            foreach (var se in entries)
                Add(se.ToSettingsItem());
        }
    }
}
