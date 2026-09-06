#nullable disable

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Graphics.Containers;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SideBar.Settings.Sections;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SideBar.Tabs;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins;
using osuTK;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SideBar.Settings
{
    public partial class PlayerSettings : OsuScrollContainer, ISidebarContent
    {
        private readonly FillFlowContainer<Section> fillFlow = new FillFlowContainer<Section>
        {
            AutoSizeAxes = Axes.Y,
            RelativeSizeAxes = Axes.X,
            Anchor = Anchor.TopRight,
            Origin = Anchor.TopRight,
            Spacing = new Vector2(5),
            Direction = FillDirection.Vertical
        };

        public LocalisableString Title => "播放器设置";
        public IconUsage Icon { get; } = FontAwesome.Solid.Cog;

        [BackgroundDependencyLoader]
        private void load(IHikariiiPluginManager pluginManager)
        {
            ScrollbarVisible = false;
            RelativeSizeAxes = Axes.Both;
            Add(fillFlow);

            foreach (var provider in pluginManager.GetAllPluginProviders().Values)
            {
                var config = pluginManager.TryGetPluginConfig<IPluginConfigManager>(provider);
                var entries = provider.GetSettingsEntries(config);

                if (entries.Length > 0)
                    AddSection(new NewPluginSettingsSection(provider.GetPluginDescription().Name, entries));
            }
        }

        private void onTabPositionChanged(ValueChangedEvent<TabControlPosition> v)
        {
            switch (v.NewValue)
            {
                case TabControlPosition.Left:
                    fillFlow.Anchor = fillFlow.Origin = Anchor.TopLeft;
                    break;

                case TabControlPosition.Right:
                    fillFlow.Anchor = fillFlow.Origin = Anchor.TopRight;
                    break;

                case TabControlPosition.Top:
                    fillFlow.Anchor = fillFlow.Origin = Anchor.TopCentre;
                    break;
            }
        }

        public void AddSection(Section section) => fillFlow.Add(section);
    }
}
