using MessagePack;
using osu.Game.Beatmaps;
using osu.Game.Users;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Screens.LLin;

[MessagePackObject(false)]
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
