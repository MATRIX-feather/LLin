using LLin.Game.Screens.Mvis.Plugins;
using LLin.Game.Screens.Mvis.SideBar.Settings.Sections;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace LLin.Game.Screens.Mvis.SideBar.PluginsPage
{
    internal class PluginsSection : Section
    {
        public override int Columns => 2;

        private MvisPluginManager manager;
        private FillFlowContainer placeholder;

        public PluginsSection()
        {
            Title = "插件";
            Icon = FontAwesome.Solid.Boxes;
        }

        protected override float PieceWidth => 300;

        [BackgroundDependencyLoader]
        private void load(MvisPluginManager pluginManager)
        {
            manager = pluginManager;
            FillFlow.Width = 600;
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

            pluginManager.OnPluginAdd += addPiece;
            pluginManager.OnPluginUnLoad += removePiece;
        }

        protected override void LoadComplete()
        {
            foreach (var pl in manager.GetAllPlugins(false))
            {
                addPiece(pl);
            }

            FillFlow.LayoutEasing = Easing.OutQuint;
            FillFlow.LayoutDuration = 250;

            base.LoadComplete();
        }

        private void addPiece(MvisPlugin plugin)
        {
            Add(new PluginPiece(plugin));

            placeholder.FadeOut(300, Easing.OutQuint);
        }

        private void removePiece(MvisPlugin plugin)
        {
            int childrenCount = 0;

            foreach (var d in FillFlow)
            {
                childrenCount += FillFlow.Children.Count;

                if (d is PluginPiece piece && piece.Plugin == plugin)
                {
                    piece.Hide();
                    break;
                }
            }

            if (childrenCount - 1 <= 0) placeholder.FadeIn(300, Easing.OutQuint);
        }
    }
}
