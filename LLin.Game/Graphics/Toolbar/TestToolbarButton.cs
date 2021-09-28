using osu.Framework.Graphics.Sprites;

namespace LLin.Game.Graphics.Toolbar
{
    public class TestToolbarButton : ToolbarButton
    {
        protected override IconUsage Icon => FontAwesome.Regular.Bell;

        public TestToolbarButton()
        {
            TooltipText = "测试！";
        }
    }
}
