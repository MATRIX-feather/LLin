using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Platform;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SettingsItems;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Config;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hikariii.Tests.HikariiiPlayerv2.test
{
    public class TestProviderA : IHikariiiPluginProvider
    {
        public string GetID() => "test-a";

        public IPluginConfigManager CreatePluginConfig(Storage storageAccess)
        {
            return new DummyPluginConfigManager();
        }

        public Type GetPluginConfigType()
        {
            return typeof(DummyPluginConfigManager);
        }

        public PluginDescription GetPluginDescription() => new("NameA", "DescriptionA", ["author1", "author2"]);

        public DrawableHikariiiPlugin CreateDrawablePlugin()
        {
            return new DrawableTestA();
        }

        public SettingsEntry[] GetSettingsEntries(IPluginConfigManager config) => [];
    }

    public partial class DrawableTestA : DrawableHikariiiPlugin
    {
        public override ContentLayerType ContentLayer => ContentLayerType.Foreground;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;
            Size = new Vector2(0.1f, 0.1f);

            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            Scale = new Vector2(0);

            Children =
            [
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black
                },
                new OsuSpriteText
                {
                    Text = "DrawableA!",
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                }
            ];
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            this.FadeInFromZero(300)
                .ScaleTo(new Vector2(1f), 300, Easing.OutQuint);
        }

        public override bool HasExitAnimation => true;

        public override void PlayExit()
        {
            base.PlayExit();

            this.FadeOut(300)
                .ScaleTo(new Vector2(0.6f), 300, Easing.OutQuint);
        }
    }
}
