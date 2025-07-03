using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osu.Game.Rulesets.Hikariii.Features.ListenerLoader.Handlers.ScreenHandlers;
using osu.Game.Screens;

namespace osu.Game.Rulesets.Hikariii.Features.ListenerLoader.Handlers;

public partial class ScreenHandlerManager : AbstractHandler
{
    private OsuScreenStack? screenStack;

    private readonly List<AbstractScreenHandler> handlers = new();

    public event Action<(IScreen last, IScreen next)>? OnScreenChanged;

    [BackgroundDependencyLoader]
    private void load()
    {
        hookScreenStack();

        this.addHandler(new NewSongSelectHandler());
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
            var screenStackField = this.FindFieldInstance(Game, typeof(OsuScreenStack));

            if (screenStackField == null) return false;

            object? val = screenStackField.GetValue(Game);

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
        if (newscreen is not Drawable drawable)
            return;

        if (!drawable.IsLoaded)
            drawable.OnLoadComplete += _ => this.processNewScreen(lastscreen, newscreen);
        else
            processNewScreen(lastscreen, newscreen);
    }

    private void processNewScreen(IScreen lastscreen, IScreen newscreen)
    {
        if (!newscreen.IsCurrentScreen())
            return;

        Logging.Log($"🦢 Screen Changed! {lastscreen} -> {newscreen}", level: LogLevel.Debug);

        foreach (var screenHandler in handlers)
            screenHandler.Handle(lastscreen, newscreen);

        OnScreenChanged?.Invoke((lastscreen, newscreen));
    }
}
