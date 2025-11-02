using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Yasp.Config;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Yasp.Panels;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Types;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Yasp
{
    public partial class YaspPlugin : BindableControlledPlugin
    {
        private Drawable? currentContent;

        /// <summary>
        /// 请参阅 <see cref="LLinPlugin.ContentLayer"/>
        /// </summary>
        public override ContentLayer Target => ContentLayer.Foreground;

        public YaspPlugin(LLinPluginProvider provider)
            : base(provider)
        {
            Name = "YASP";

            Flags.AddRange([
                PluginFlags.CanDisable
            ]);

            RelativeSizeAxes = Axes.Both;
        }

        private readonly Bindable<PanelType> panelType = new Bindable<PanelType>();

        private WorkingBeatmap? currentWorkingBeatmap;

        /// <summary>
        /// 请参阅 <see cref="LLinPlugin.CreateContent()"/>
        /// </summary>
        protected override Drawable CreateContent()
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
        protected override bool OnContentLoaded(Drawable content)
        {
            currentContent?.Hide();
            currentContent?.Expire();

            currentContent = content;
            content.Alpha = 0.01f;
            content.Hide();

            if (!Enabled.Value) return true;

            content.Show();
            refresh();

            return true;
        }

        public override bool Disable()
        {
            currentContent?.Hide();
            return base.Disable();
        }

        public override bool Enable()
        {
            bool result = base.Enable();

            LLin!.OnBeatmapChanged(onBeatmapChanged, this, true);
            currentContent?.Show();
            refresh();

            return result;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            var config = (YaspConfigManager)Dependencies.Get<LLinPluginManager>().GetConfigManager(Provider.Identifier());
            config.BindWith(YaspSettings.EnablePlugin, Enabled);
            config.BindWith(YaspSettings.PanelType, panelType);

            panelType.BindValueChanged(v =>
            {
                Load();
            }, true);
        }

        protected override bool PostInit()
        {
            currentWorkingBeatmap ??= LLin?.Beatmap.Value;
            return true;
        }

        private void refresh()
        {
            if (currentContent is IPanel panel)
                panel.Refresh(currentWorkingBeatmap!);
            else if (currentContent != null)
                Logging.LogError(new InvalidCastException("CurrentContent不是IPanel?"));
        }

        private void onBeatmapChanged(WorkingBeatmap working)
        {
            if (Disabled.Value) return;

            if (currentWorkingBeatmap == working) return;

            currentWorkingBeatmap = working;
            refresh();
        }
    }
}
