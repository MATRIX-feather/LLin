#nullable disable

using System;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Interfaces.Plugins;
using osuTK;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Types
{
    public class ToggleableButtonWrapper : IToggleableFunctionProvider
    {
        public Vector2 Size { get; set; } = new Vector2(30);
        public Func<bool> Action { get; set; }
        public IconUsage Icon { get; set; }
        public LocalisableString Title { get; set; }
        public LocalisableString Description { get; set; }
        public FunctionType Type { get; set; }

        public bool Active()
        {
            if (Bindable.Disabled) return false;

            Bindable.Toggle();

            bool success = Action?.Invoke() ?? false;
            OnActive?.Invoke(success);

            return success;
        }

        public Action<bool> OnActive { get; set; }

        public BindableBool Bindable { get; set; } = new BindableBool();
    }
}
