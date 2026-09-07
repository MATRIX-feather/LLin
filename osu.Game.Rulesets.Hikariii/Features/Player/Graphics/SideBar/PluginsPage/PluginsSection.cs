using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SideBar.Settings.Sections;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SideBar.PluginsPage
{
    internal partial class PluginsSection : Section
    {
        [Resolved]
        private SessionPluginManager manager { get; set; } = null!;

        [Resolved]
        private IHikariiiPluginManager globalPlugins { get; set; }

        private FillFlowContainer? placeholder;

        public PluginsSection()
        {
            Title = "插件";
            Icon = FontAwesome.Solid.Boxes;
        }

        private readonly IDictionary<string, PluginPiece> pluginPieces = new Dictionary<string, PluginPiece>();

        [BackgroundDependencyLoader]
        private void load()
        {
            FillFlow.RelativeSizeAxes = Axes.None;
            FillFlow.AutoSizeAxes = Axes.Both;
            FillFlow.Direction = FillDirection.Vertical;

            AddInternal(placeholder = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Colour = Color4.White.Opacity(0.6f),
                Margin = new MarginPadding(40),
                Children = new Drawable[]
                {
                    new SpriteIcon
                    {
                        Icon = FontAwesome.Solid.Boxes,
                        Size = new Vector2(60),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                    },
                    new OsuSpriteText
                    {
                        Text = "没有插件",
                        Font = OsuFont.GetFont(size: 45, weight: FontWeight.Bold),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                    }
                }
            });

            manager.OnPluginEnable += pair => activate(pair.id);
            manager.OnPluginDisable += pair => deactivate(pair.id);
        }

        protected override void LoadComplete()
        {
            foreach (var pl in globalPlugins.GetAllPluginProviders())
            {
                string id = pl.Key;
                PluginPiece pluginPiece;

                Add(pluginPiece = new PluginPiece(id)
                {
                    PluginDisabled = { Value = !manager.IsPluginEnabled(id) }
                });
                pluginPieces[id] = pluginPiece;
            }

            if (pluginPieces.Count != 0)
                placeholder.FadeOut(300, Easing.OutQuint);

            FillFlow.LayoutEasing = Easing.OutQuint;
            FillFlow.LayoutDuration = 250;

            base.LoadComplete();
        }

        private void activate(string id)
        {
            bool hasExistingPiece = pluginPieces.TryGetValue(id, out PluginPiece pluginPiece);

            if (!hasExistingPiece)
                return;

            pluginPiece!.PluginDisabled.Value = false;
        }

        private void deactivate(string id)
        {
            pluginPieces.TryGetValue(id, out PluginPiece pluginPiece);
            if (pluginPiece == null) return;

            pluginPiece.PluginDisabled.Value = true;
        }
    }
}
