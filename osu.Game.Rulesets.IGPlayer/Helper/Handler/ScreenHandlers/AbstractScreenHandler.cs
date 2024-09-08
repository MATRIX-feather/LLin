using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace osu.Game.Rulesets.IGPlayer.Helper.Handler.ScreenHandlers;

public abstract partial class AbstractScreenHandler : Drawable
{
    [Resolved]
    private OsuGame game { get; set; } = null!;

    protected OsuGame Game => game;

    public abstract void Handle(IScreen prev, IScreen next);

    protected ScreenStack? ScreenStack { get; set; }

    public void SetScreenStack(ScreenStack screenStack)
    {
        this.ScreenStack = screenStack;
    }
}
