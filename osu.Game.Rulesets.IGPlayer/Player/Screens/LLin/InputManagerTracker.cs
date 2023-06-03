using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osu.Framework.Logging;

namespace osu.Game.Rulesets.IGPlayer.Player.Screens.LLin;

public partial class InputManagerTracker : CompositeDrawable
{
    [BackgroundDependencyLoader]
    private void load()
    {
        focusedDrawable.BindValueChanged(this.onFocusChange);
    }

    private InputManager? inputManager;

    private readonly Bindable<Drawable> focusedDrawable = new();

    private readonly List<object> focusHistory = new();

    private void onFocusChange(ValueChangedEvent<Drawable> e)
    {
        Drawable? newDrawable = e.NewValue;

        Logger.Log($"🦢 Adding {newDrawable} to List {focusHistory.Count}");
        focusHistory.Add(newDrawable);
        Logger.Log($"🦢 New Count: {focusHistory.Count}");

        if (focusHistory.Count > 3)
            focusHistory.RemoveAt(0);
    }

    // AnotherFocus -> null -> Player <<-- Block first input
    // Player -> null (L/R Click) <<-- Don't block
    public bool ShouldBlockFirstInput()
    {
        var count = focusHistory.Count;
        //Logger.Log($"🦢 List size: {focusHistory.Count}");

        var index = count - 1;

        return count switch
        {
            >= 3 =>
                //Logger.Log($"🦢 Last 3 History: {focusHistory[index]} -->> {focusHistory[index - 1]} -->> {focusHistory[index - 2]}");
                focusHistory[index] != null && focusHistory[index - 2] is LLinScreen,
            >= 2 =>
                //Logger.Log($"🦢 Last 3 History: {focusHistory[index]} -->> {focusHistory[index - 1]}");
                focusHistory[index] != null && focusHistory[index - 1] is LLinScreen,
            _ => false
        };
    }

    public void ClearHistory()
    {
        focusHistory.Clear();
    }

    protected override void Update()
    {
        base.Update();

        if (inputManager == null)
            this.inputManager = GetContainingInputManager();

        if (inputManager.FocusedDrawable != focusedDrawable.Value)
            focusedDrawable.Value = inputManager.FocusedDrawable;
    }
}
