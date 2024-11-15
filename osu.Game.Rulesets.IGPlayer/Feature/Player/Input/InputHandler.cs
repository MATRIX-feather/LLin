using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Screens.LLin;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Input;

public partial class RulesetInputHandler : CompositeDrawable, IKeyBindingHandler<HikariiiAction>
{
    public RulesetInputHandler(Dictionary<HikariiiAction, Action> keybinds, LLinScreen screen)
    {
        this.keyBinds = keybinds;
        this.screen = screen;
    }

    public bool HandleExternal(UIEvent e)
    {
        return this.Handle(e);
    }

    private readonly LLinScreen screen;

    private readonly Dictionary<HikariiiAction, Action> keyBinds;

    public void RegisterAction(HikariiiAction action)
    {
    }

    public bool BlockNextAction;

    public bool OnPressed(KeyBindingPressEvent<HikariiiAction> action)
    {
        Logging.Log("Got event " + action + "!!!");

        if (BlockNextAction)
        {
            BlockNextAction = false;
            return true;
        }

        if (!screen.HasFocus) return true;

        //查找本体按键绑定
        var target = keyBinds.FirstOrDefault(b => b.Key == action.Action).Value;
        target?.Invoke();

        return target != null;
    }

    public void OnReleased(KeyBindingReleaseEvent<HikariiiAction> e)
    {
    }
}
