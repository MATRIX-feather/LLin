using osu.Game.Beatmaps;
using osu.Game.Users;

namespace osu.Game.Rulesets.IGPlayer.Player;

public class InPlayerUserActivity : UserActivity.InGame
{
    public override string GetStatus(bool hideIdentifiableInformation = false)
    {
        return "正在听歌";
    }

    public InPlayerUserActivity(IBeatmapInfo beatmapInfo, IRulesetInfo ruleset)
        : base(beatmapInfo, ruleset)
    {
    }
}
