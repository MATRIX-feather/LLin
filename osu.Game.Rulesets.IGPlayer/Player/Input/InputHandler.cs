using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace osu.Game.Rulesets.IGPlayer.Player.Input;

public partial class RulesetInputHandler : CompositeDrawable, IKeyBindingHandler<IGAction>
{
    public RulesetInputHandler(Dictionary<IGAction, Action> keybinds)
    {
        this.keyBinds = keybinds;
    }

    private readonly Dictionary<IGAction, Action> keyBinds;

    public bool BlockNextAction;

    public bool OnPressed(KeyBindingPressEvent<IGAction> action)
    {
        if (BlockNextAction)
        {
            BlockNextAction = false;
            return true;
        }

        //查找本体按键绑定
        var target = keyBinds.FirstOrDefault(b => b.Key == action.Action).Value;
        target?.Invoke();

        return target != null;
    }

    public void OnReleased(KeyBindingReleaseEvent<IGAction> e)
    {
    }
}
