using osu.Framework.Graphics.Containers;

namespace LLin.Game.Graphics
{
    public abstract class BackgroundOverlayContainer : Container
    {
        public abstract void OnPopOut();
        public abstract void OnPopIn();
        public virtual bool ExpireAfterPopOut { get; protected set; } = false;
    }
}
