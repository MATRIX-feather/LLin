using System;
using System.Linq;
using System.Reflection;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Screens.LLin;
using osu.Game.Screens.Menu;
using osu.Game.Screens.Select;
using osuTK.Graphics;

namespace osu.Game.Rulesets.IGPlayer.Helper.Handler.ScreenHandlers;

public partial class PlaySongSelectHandler : AbstractScreenHandler
{
    private PlaySongSelect? currentPlaySongSelect;

    private partial class FooterButtonOpenInMvis : FooterButton
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            Alpha = 0;
            SelectedColour = new Color4(0, 86, 73, 255);
            DeselectedColour = SelectedColour.Opacity(0.5f);
            Text = @"在Hikariii中打开";
        }

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();

            ButtonContentContainer.Margin = new MarginPadding { Horizontal = (100 - TextContainer.Width) / 2 };
        }
    }

    public override void Handle(IScreen prev, IScreen next)
    {
        if (prev == currentPlaySongSelect && next is MainMenu)
            currentPlaySongSelect = null;

        if (next is not PlaySongSelect playSongSelect) return;

        if (playSongSelect == currentPlaySongSelect) return;

        try
        {
            if (ScreenStack!.CurrentScreen != playSongSelect) return;

            const BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.SetProperty
                                      | BindingFlags.SetField;

            var property = playSongSelect.GetType().GetProperties(flag)
                                         .FirstOrDefault(f => f.PropertyType == typeof(Footer)); //?.GetValue(playSongSelect);

            if (property == null)
            {
                Logging.Log("没有找到Footer属性", level: LogLevel.Important);
                return;
            }

            object? obj = property.GetValue(playSongSelect);

            if (obj is not Footer footer)
            {
                Logging.Log("Footer为null", level: LogLevel.Important);
                return;
            }

            footer.AddButton(new FooterButtonOpenInMvis
            {
                Action = this.pushPlayerScreen
            }, null);

            currentPlaySongSelect = playSongSelect;
        }
        catch (Exception e)
        {
            Logging.LogError(e, "向歌曲选择添加入口时出现问题");
        }
    }

    private void pushPlayerScreen()
    {
        Game.PerformFromScreen(s => s.Push(new LLinScreen()), new[]
        {
            typeof(MainMenu),
            typeof(PlaySongSelect)
        });
    }
}
