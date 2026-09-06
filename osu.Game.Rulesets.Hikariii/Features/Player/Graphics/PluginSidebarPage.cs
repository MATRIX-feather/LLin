using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Framework.Logging;
using osu.Game.Online.Placeholders;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SideBar;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Extensions;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins;
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
        protected virtual void InitContent(DrawableHikariiiPlugin plugin)
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
        public DrawableHikariiiPlugin Plugin { get; }

        /// <summary>
        /// 插件的ConfigManager
        /// </summary>
        protected IPluginConfigManager Config => Dependencies.Get<IHikariiiPluginManager>().TryGetPluginConfigOrThrow<IPluginConfigManager>(PluginId);

        [Resolved(canBeNull: true)]
        private SessionPluginManager? pluginManager { get; set; }

        protected PluginSidebarPage(string id)
        {
            PluginId = id;
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
                    Action = () => pluginManager?.EnablePlugin(id),
                    Scale = new Vector2(1.25f)
                }
            ];
        }

        public string PluginId { get; set; }

        private DependencyContainer dependencies;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        [Resolved]
        private CustomColourProvider colourProvider { get; set; }

        [Resolved]
        private IHikariiiPluginManager plugins { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            Title = plugins.GetPluginProviderOrThrow(PluginId).GetPluginDescription().Name;

            dependencies.Cache(this);
            dependencies.Cache(Plugin);
            dependencies.Cache(Dependencies.Get<IHikariiiPluginManager>().TryGetPluginConfigOrThrow<IPluginConfigManager>(PluginId));

            Logging.Log(level: LogLevel.Important, message: "FIXME: fix sidebar page class");
            //todo: FIXME: watch plugin state then implement "enable this plugin!" hint.
            /*Plugin.Disabled.BindValueChanged(v =>
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
            }, true);*/
        }

        public LocalisableString Title { get; private set; }
        public IconUsage Icon { get; set; } = FontAwesome.Solid.Plug;
    }
}
