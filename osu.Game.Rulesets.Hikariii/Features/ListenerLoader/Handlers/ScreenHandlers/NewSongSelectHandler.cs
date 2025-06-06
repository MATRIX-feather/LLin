using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Screens;
using osu.Game.Graphics;
using osu.Game.Rulesets.Hikariii.Features.Configuration;
using osu.Game.Rulesets.Hikariii.Features.Player.Screens.LLin;
using osu.Game.Screens.Footer;
using osu.Game.Screens.Menu;
using osu.Game.Screens.Select;
using osu.Game.Screens.SelectV2;

namespace osu.Game.Rulesets.Hikariii.Features.ListenerLoader.Handlers.ScreenHandlers;

public partial class NewSongSelectHandler : AbstractScreenHandler
{
    private partial class NewFooterButtonOpenInMvis : ScreenFooterButton
    {
        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            Text = @"在Hikariii中打开";
            AccentColour = colours.Lime1;
            Icon = FontAwesome.Solid.Play;
        }
    }

    [Resolved]
    private ScreenFooter screenFooter { get; set; } = null!;

    private readonly BindableBool enableInject = new();

    [BackgroundDependencyLoader]
    private void load(MConfigManager config)
    {
        config.BindWith(MSetting.InjectButtonToNewSongSelect, enableInject);
    }

    public override void Handle(IScreen prev, IScreen next)
    {
        if (!enableInject.Value)
            return;

        if (next is not SoloSongSelect playSongSelect) return;

        waitUntilSelectReady(playSongSelect, () => injectButtons(playSongSelect));
    }

    private void injectButtons(SoloSongSelect songSelect)
    {
        if (!songSelect.IsCurrentScreen())
            return;

        try
        {
            /*
            const BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            var gameFooterField = Game.GetType().GetField("ScreenFooter", flag);
            if (gameFooterField is null)
                throw new MissingFieldException("ScreenFooter");

            if (gameFooterField.GetValue(Game) is not ScreenFooter screenFooter)
                throw new NullDependencyException("OsuGame.ScreenFooter is not a instance of ScreenFooter!");
            */

            List<ScreenFooterButton> buttons = [];

            buttons.AddRange(songSelect.CreateFooterButtons());
            buttons.Add(new NewFooterButtonOpenInMvis
            {
                Action = this.pushPlayerScreen
            });

            screenFooter.SetButtons(buttons);
        }
        catch (Exception e)
        {
            Logging.LogError(e, "向歌曲选择添加入口时出现问题");
        }
    }

    private void waitUntilSelectReady(SoloSongSelect songSelect, Action action)
    {
        if (!songSelect.IsLoaded)
            this.Delay(100).Schedule(() => waitUntilSelectReady(songSelect, action));

        this.Delay(1000).Schedule(action);

        //action.Invoke();
    }

    private void pushPlayerScreen()
    {
        Game.PerformFromScreen(s => s.Push(new LLinScreen()), new[]
        {
            typeof(MainMenu),
            typeof(PlaySongSelect),
            typeof(SoloSongSelect)
        });
    }
}
