using System;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Types
{
    public class ButtonWrapper : IFunctionProvider
    {
        public Vector2 Size { get; set; } = new Vector2(30);
        public Func<bool>? Action { get; set; }
        public IconUsage Icon { get; set; }
        public LocalisableString Title { get; set; }
        public LocalisableString Description { get; set; }
        public FunctionType Type { get; set; }

        public virtual bool Active()
        {
            bool success = Action?.Invoke() ?? false;

            OnActive?.Invoke(success);

            return success;
        }

        public Action<bool>? OnActive { get; set; }
    }
}
