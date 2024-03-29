#nullable disable

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Interfaces.Plugins;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Types
{
    public abstract partial class BindableControlledPlugin : LLinPlugin
    {
        [Resolved]
        private LLinPluginManager manager { get; set; }

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
            if (Enabled.Value && !playerExiting)
                manager.ActivePlugin(this);
            else
                manager.DisablePlugin(this);
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
