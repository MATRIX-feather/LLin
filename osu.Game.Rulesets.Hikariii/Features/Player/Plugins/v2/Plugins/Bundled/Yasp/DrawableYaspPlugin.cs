using System;
using System.Threading;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Extensions;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Bundled.Yasp.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Bundled.Yasp.Panels;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins.Bundled.Yasp
{
    public partial class DrawableYaspPlugin : DrawableHikariiiPlugin
    {
        private Drawable? currentContent;

        public override ContentLayerType ContentLayer => ContentLayerType.Foreground;

        public DrawableYaspPlugin(IHikariiiPluginProvider provider)
        {
            this.Provider = provider;
            Name = "YASP";
            RelativeSizeAxes = Axes.Both;
        }

        private readonly Bindable<PanelType> panelType = new Bindable<PanelType>();

        private WorkingBeatmap? currentWorkingBeatmap;
        public readonly IHikariiiPluginProvider Provider;

        private Drawable createContent()
        {
            Drawable target = panelType.Value switch
            {
                PanelType.Classic => new ClassicPanel(),
                PanelType.SongCover => new NsiPanel(),
                PanelType.CoverII => new CoverIIPanel(),
                _ => throw new InvalidOperationException($"未知的PanelType: {panelType.Value}")
            };

            return target;
        }

        /// <summary>
        /// 请参阅 <see cref="LLinPlugin.OnContentLoaded(Drawable)"/>
        /// </summary>
        private void onContentLoaded(Drawable content)
        {
            currentContent?.Hide();
            currentContent?.Expire();

            currentContent = content;
            content.Alpha = 0.01f;
            content.Hide();

            content.Show();
            Add(content);
            refreshPanel();
        }

        public override bool HasExitAnimation => true;

        private DependencyContainer dependencies;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
            => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        private CancellationTokenSource? cancellationTokenSource;

        [BackgroundDependencyLoader]
        private void load(IHikariiiPluginManager pluginManager)
        {
            var config = pluginManager.TryGetPluginConfigOrThrow<YaspConfigManager>(Provider.GetID());
            config.BindWith(YaspSettings.PanelType, panelType);

            dependencies.Cache(config);

            panelType.BindValueChanged(v =>
            {
                cancellationTokenSource?.Cancel();
                cancellationTokenSource = new CancellationTokenSource();

                this.LoadComponentAsync(createContent(), onContentLoaded, cancellationTokenSource.Token);
            }, true);
        }

        protected override void LoadComplete()
        {
            currentWorkingBeatmap ??= Player.Beatmap.Value;
            Player.OnBeatmapChanged(onBeatmapChanged, this, true);
            currentContent?.Show();
            refreshPanel();

            base.LoadComplete();
        }

        private void refreshPanel()
        {
            if (currentContent is IPanel panel)
                panel.Refresh(currentWorkingBeatmap!);
            else if (currentContent != null)
                Logging.LogError(new InvalidCastException("CurrentContent不是IPanel?"));
        }

        private void onBeatmapChanged(WorkingBeatmap working)
        {
            if (currentWorkingBeatmap == working) return;

            currentWorkingBeatmap = working;
            refreshPanel();
        }

        public override void PlayExit()
        {
            this.FadeOut(300);
            currentContent?.Hide();
            base.PlayExit();
        }
    }
}
