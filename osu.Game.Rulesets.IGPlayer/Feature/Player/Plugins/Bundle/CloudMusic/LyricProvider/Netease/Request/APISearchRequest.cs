using System;
using osu.Game.Online.API;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.LyricProvider.Netease.Response;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.LyricProvider.Netease.Request
{
    public class APISearchRequest : OsuJsonWebRequest<APISearchResponseRoot>
    {
        public APISearchRequest(string target)
        {
            Url = $"https://music.163.com/api/search/get/web?hlpretag=&hlposttag=&s={target}&type=1&total=true&limit=1";
        }

        protected override void ProcessResponse()
        {
            try
            {
                base.ProcessResponse();
            }
            catch (Exception e)
            {
                Logging.LogError(e, "无法将返回的内容转换为APISearchResponseRoot, 上游返回了意外的结果");
            }
        }
    }
}
