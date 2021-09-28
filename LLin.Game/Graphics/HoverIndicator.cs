using System;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;

namespace LLin.Game.Graphics
{
    public class HoverIndicator : CompositeDrawable
    {
        public event Action Hover;
        public event Action HoverLost;

        protected override bool OnHover(HoverEvent e)
        {
            Hover?.Invoke();
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            HoverLost?.Invoke();
            base.OnHoverLost(e);
        }
    }
}
