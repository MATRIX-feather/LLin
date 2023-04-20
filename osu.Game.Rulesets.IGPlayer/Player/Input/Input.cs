using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace osu.Game.Rulesets.IGPlayer.Player.Input;

public partial class InputHandler : CompositeDrawable, IKeyBindingHandler<IGAction>
{
    public InputHandler(Dictionary<IGAction, Action> keybinds)
    {
        this.keyBinds = keybinds;
    }

    private readonly Dictionary<IGAction, Action> keyBinds;

    public bool OnPressed(KeyBindingPressEvent<IGAction> action)
    {
        //查找本体按键绑定
        var target = keyBinds.FirstOrDefault(b => b.Key == action.Action).Value;
        target?.Invoke();

        return target != null;
    }

    public void OnReleased(KeyBindingReleaseEvent<IGAction> e)
    {
    }
}
