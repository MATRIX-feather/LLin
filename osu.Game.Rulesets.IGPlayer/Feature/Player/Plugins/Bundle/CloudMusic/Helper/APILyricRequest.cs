using osu.Game.Online.API;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.Misc;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.Helper
{
    public class APILyricRequest : OsuJsonWebRequest<APILyricResponseRoot>
    {
        public APILyricRequest(int id)
        {
            Url = $"https://music.163.com/api/song/lyric?os=pc&id={id}&lv=-1&kv=-1&tv=-1";
        }
    }
}
