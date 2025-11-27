using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Online.Placeholders;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SideBar;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Screens.LLin;
using osuTK;
using osuTK.Input;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Graphics
{
    public abstract partial class PluginSidebarPage : Container, ISidebarContent
    {
        private readonly ClickablePlaceholder placeholder;
        private readonly Container content;

        protected override Container<Drawable> Content => content;

        /// <summary>
        /// 初始化内容使用
        /// </summary>
        /// <param name="plugin">初始化内容所使用的插件</param>
        protected virtual void InitContent(LLinPlugin plugin)
        {
        }

        /// <summary>
        /// 获取侧边栏入口
        /// </summary>
        /// <returns>一个侧边栏插件功能控制器</returns>
        public virtual IPluginFunctionProvider? GetFunctionEntry() => null;

        /// <summary>
        /// 激活快捷键
        /// </summary>
        public virtual Key ShortcutKey => Key.Unknown;

        private bool contentInit;

        /// <summary>
        /// 源插件
        /// </summary>
        public LLinPlugin Plugin { get; }

        /// <summary>
        /// 插件的ConfigManager
        /// </summary>
        protected IPluginConfigManager Config => Dependencies.Get<LLinPluginManager>().GetConfigManager(Plugin.Provider);

        [Resolved(canBeNull: true)]
        private SessionPluginManager? pluginManager { get; set; }

        protected PluginSidebarPage(LLinPlugin plugin)
        {
            Plugin = plugin;
            Title = plugin.Name;
            RelativeSizeAxes = Axes.Both;

            InternalChildren =
            [
                content = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0
                },
                placeholder = new ClickablePlaceholder("请先启用该插件!", FontAwesome.Solid.Plug)
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Action = () => pluginManager?.EnablePlugin(Plugin),
                    Scale = new Vector2(1.25f)
                }
            ];
        }

        private DependencyContainer dependencies;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        [Resolved]
        private CustomColourProvider colourProvider { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            dependencies.Cache(this);
            dependencies.Cache(Plugin);
            dependencies.Cache(Dependencies.Get<LLinPluginManager>().GetConfigManager(Plugin.Provider));

            Plugin.Disabled.BindValueChanged(v =>
            {
                if (v.NewValue)
                {
                    content.FadeOut();
                    placeholder.MoveToY(0, 200, Easing.OutQuint).FadeIn(200, Easing.OutQuint);
                }
                else
                {
                    content.FadeIn(200, Easing.OutQuint);
                    placeholder.MoveToY(30, 200, Easing.OutQuint).FadeOut(200, Easing.OutQuint);
                }

                if (!v.NewValue && !contentInit)
                {
                    InitContent(Plugin);
                    contentInit = true;
                }
            }, true);
        }

        public LocalisableString Title { get; }
        public IconUsage Icon { get; set; } = FontAwesome.Solid.Plug;
    }
}
