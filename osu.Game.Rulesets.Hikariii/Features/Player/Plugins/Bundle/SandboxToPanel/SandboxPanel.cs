#nullable disable

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.SandboxToPanel.RulesetComponents.Configuration;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Types;
using osuTK;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.SandboxToPanel
{
    using LayoutController = RulesetComponents.Screens.Visualizer.Components.LayoutController;
    using Particles = RulesetComponents.Screens.Visualizer.Components.Particles;

    [Cached]
    public partial class SandboxPlugin : BindableControlledPlugin
    {
        public override LLinPlugin.ContentLayer Target => LLinPlugin.ContentLayer.Foreground;
        public Bindable<WorkingBeatmap> CurrentBeatmap = new Bindable<WorkingBeatmap>();

        public SandboxPlugin(LLinPluginProvider provider)
            : base(provider)
        {
            Name = "Sandbox";

            Flags.AddRange([
                PluginFlags.CanDisable
            ]);

            RelativeSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Scale = new Vector2(0.8f);
        }

        private readonly BindableFloat idleAlpha = new BindableFloat();

        [BackgroundDependencyLoader]
        private void load()
        {
            idleAlpha.BindValueChanged(onIdleAlphaChanged);

            var config = (SandboxRulesetConfigManager)Dependencies.Get<LLinPluginManager>().GetConfigManager(Provider.Identifier());

            config.BindWith(SandboxRulesetSetting.EnableRulesetPanel, Enabled);
            config.BindWith(SandboxRulesetSetting.IdleAlpha, idleAlpha);

            if (LLin != null)
            {
                LLin.OnIdle += () => idleAlpha.TriggerChange();
                LLin.OnActive += () =>
                {
                    if (Enabled.Value)
                        this.FadeTo(1, 750, Easing.OutQuint);

                    CurrentBeatmap.Disabled = false;
                    LLin?.OnBeatmapChanged(onBeatmapChanged, this, true);
                };
            }
        }

        private void onIdleAlphaChanged(ValueChangedEvent<float> v)
        {
            if ((LLin?.IsIdle ?? true) && Enabled.Value)
            {
                this.FadeTo(v.NewValue, 750, Easing.OutQuint);
                if (v.NewValue == 0) CurrentBeatmap.Disabled = true;
            }
        }

        protected override Drawable CreateContent() => new Container
        {
            RelativeSizeAxes = Axes.Both,
            Children = new Drawable[]
            {
                new Particles(),
                new LayoutController()
            }
        };

        protected override bool OnContentLoaded(Drawable content) => true;

        protected override bool PostInit() => true;

        public override bool Disable()
        {
            this.FadeOut(300, Easing.OutQuint).ScaleTo(0.8f, 400, Easing.OutQuint);

            return base.Disable();
        }

        public override bool Enable()
        {
            bool result = base.Enable();

            this.FadeTo(LLin?.IsIdle ?? false ? idleAlpha.Value : 1, 300).ScaleTo(1, 400, Easing.OutQuint);
            LLin?.OnBeatmapChanged(onBeatmapChanged, this, true);

            return result;
        }

        private void onBeatmapChanged(WorkingBeatmap working)
        {
            if (Disabled.Value || CurrentBeatmap.Disabled) return;

            CurrentBeatmap.Value = working;
        }

        public override void UnLoad()
        {
            if (ContentLoaded)
            {
                //MvisScreen.OnScreenExiting -= beatmapLogo.StopResponseOnBeatmapChanges;
                //MvisScreen.OnScreenSuspending -= beatmapLogo.StopResponseOnBeatmapChanges;
            }

            Enabled.UnbindAll();
            Disable();

            //bug: 直接调用Expire会导致面板直接消失
            this.Delay(400).Expire();
        }
    }
}
