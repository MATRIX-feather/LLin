#nullable disable

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Screens.LLin;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Types
{
    [Obsolete]
    public abstract partial class BindableControlledPlugin(LLinPluginProvider provider) : LLinPlugin(provider)
    {
        [Resolved]
        private SessionPluginManager manager { get; set; }

        protected BindableBool Enabled = new BindableBool();
        private bool playerExiting;

        [BackgroundDependencyLoader]
        private void load()
        {
            if (LLin != null)
                LLin.Exiting += () => playerExiting = true;
        }

        protected override void LoadComplete()
        {
            Enabled.BindValueChanged(OnValueChanged, true);
            base.LoadComplete();
        }

        protected virtual void OnValueChanged(ValueChangedEvent<bool> v)
        {
            throw new NotImplementedException();
        }

        public override bool Disable()
        {
            Enabled.Value = false;
            return base.Disable();
        }

        public override bool Enable()
        {
            Enabled.Value = true;
            return base.Enable();
        }

        public override void UnLoad()
        {
            Enabled.UnbindAll();
            base.UnLoad();
        }
    }
}
