using System.Reflection;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Game.Tests;
using SDL;

namespace osu.Game.Rulesets.Hikariii.Tests;

public partial class HikariiiTestBrowser : OsuTestBrowser
{
    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
    {
        var newDeps = new DependencyContainer(base.CreateChildDependencies(parent));

        newDeps.Cache(this);

        return newDeps;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        SystemCursorVisible.BindValueChanged(v =>
        {
            Logging.Log("Hey! " + v.NewValue);

            if (v.NewValue)
            {
                SDL3.SDL_ShowCursor();
                SDL2.SDL.SDL_ShowCursor(1);
            }
            else
            {
                SDL3.SDL_HideCursor();
                SDL2.SDL.SDL_ShowCursor(0);
            }
        });
    }

    protected override void Update()
    {
        base.Update();
        MaintainCursorVisibility();
    }

    public BindableBool OsuHostCursorVisible { get; } = new BindableBool(true);
    public BindableBool SystemCursorVisible { get; } = new BindableBool();

    public void MaintainCursorVisibility()
    {
        bool hostCursorVisible = OsuHostCursorVisible.Value;

        var field = GlobalCursorDisplay.GetType().GetField("ShowCursor", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
        if (field == null)
            throw new NullDependencyException("No ShowCursor found!");

        bool? currentGameCursorState = field.GetValue(GlobalCursorDisplay) as bool?;

        if (hostCursorVisible == currentGameCursorState)
            return;

        field.SetValue(GlobalCursorDisplay, hostCursorVisible);
    }
}
