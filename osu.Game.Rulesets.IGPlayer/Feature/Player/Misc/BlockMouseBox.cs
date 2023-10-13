using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Misc
{
    public partial class BlockMouseBox : Box
    {
        protected override bool OnClick(ClickEvent e) => true;
        protected override bool OnMouseMove(MouseMoveEvent e) => true;
        protected override bool OnMouseDown(MouseDownEvent e) => true;
    }
}
