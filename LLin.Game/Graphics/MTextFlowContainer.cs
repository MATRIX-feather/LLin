using osu.Framework.Graphics.Containers;

namespace LLin.Game.Graphics
{
    public class MTextFlowContainer : TextFlowContainer<MSpriteText>
    {
        protected override MSpriteText CreateSpriteText() => new MSpriteText();
    }
}
