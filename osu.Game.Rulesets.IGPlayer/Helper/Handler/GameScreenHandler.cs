using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osu.Game.Rulesets.IGPlayer.Helper.Handler.ScreenHandlers;
using osu.Game.Screens;

namespace osu.Game.Rulesets.IGPlayer.Helper.Handler;

public partial class GameScreenHandler : AbstractInjector
{
    private OsuScreenStack? screenStack;

    [Resolved]
    private OsuGame game { get; set; } = null!;

    [Resolved(canBeNull: true)]
    private IBindable<RulesetInfo>? ruleset { get; set; }

    private readonly List<AbstractScreenHandler> handlers = new();

    [BackgroundDependencyLoader]
    private void load()
    {
        hookScreenStack();

        this.addHandler(new PlaySongSelectHandler());

        if (ruleset is Bindable<RulesetInfo> rs)
        {
            //避免用户切换到此ruleset
            ruleset?.BindValueChanged(v =>
            {
                if (v.NewValue.ShortName == "igplayerruleset" && v.OldValue != null && v.OldValue.ShortName != "igplayerruleset")
                    rs.Value = v.OldValue;
            });
        }
    }

    private void addHandler(AbstractScreenHandler handler)
    {
        this.AddInternal(handler);
        this.handlers.Add(handler);

        if (this.screenStack != null)
            handler.SetScreenStack(this.screenStack);
    }

    private bool hookScreenStack()
    {
        lock (this)
        {
            var screenStackField = this.FindFieldInstance(game, typeof(OsuScreenStack));

            if (screenStackField == null) return false;

            object? val = screenStackField.GetValue(game);

            if (val is not OsuScreenStack osuScreenStack) return false;

            screenStack = osuScreenStack;

            screenStack.ScreenExited += onScreenSwitch;
            screenStack.ScreenPushed += onScreenSwitch;

            foreach (var abstractScreenHandler in this.handlers)
                abstractScreenHandler.SetScreenStack(osuScreenStack);

            return true;
        }
    }

    private void onScreenSwitch(IScreen lastscreen, IScreen newscreen)
    {
        if (newscreen is Drawable drawable)
            drawable.OnLoadComplete += _ => this.processNewScreen(lastscreen, newscreen);
        else
            processNewScreen(lastscreen, newscreen);
    }

    private void processNewScreen(IScreen lastscreen, IScreen newscreen)
    {
        Logging.Log($"🦢 Screen Changed! {lastscreen} -> {newscreen}", level: LogLevel.Debug);

        foreach (var screenHandler in handlers)
            screenHandler.Handle(lastscreen, newscreen);
    }
}
