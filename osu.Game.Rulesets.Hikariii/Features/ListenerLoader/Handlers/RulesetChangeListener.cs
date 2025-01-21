using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Screens;
using osu.Game.Rulesets.Hikariii.Features.Player.Screens.LLin;
using osu.Game.Screens.Menu;
using osu.Game.Screens.Select;

namespace osu.Game.Rulesets.Hikariii.Features.ListenerLoader.Handlers;

public partial class RulesetChangeListener : AbstractHandler
{
    [Resolved(canBeNull: true)]
    private IBindable<RulesetInfo>? ruleset { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        if (ruleset is Bindable<RulesetInfo> rs)
        {
            //用户切换到此Ruleset时转到播放器界面
            ruleset?.BindValueChanged(v =>
            {
                if (v.NewValue.ShortName != HikariiiPlayerRuleset.SHORT_NAME
                    || v.OldValue == null
                    || v.OldValue.ShortName == HikariiiPlayerRuleset.SHORT_NAME)
                {
                    return;
                }

                Game.PerformFromScreen(screen => screen.Push(new LLinScreen()), [typeof(MainMenu), typeof(PlaySongSelect)]);
                rs.Value = v.OldValue;
            });
        }
    }
}
