using osu.Game.Online;

namespace LLin.Game.Online
{
    public class VoidApiEndpointConfiguration : EndpointConfiguration
    {
        public VoidApiEndpointConfiguration()
        {
            WebsiteRootUrl = APIEndpointUrl = string.Empty;
            APIClientSecret = string.Empty;
            APIClientID = string.Empty;
            SpectatorEndpointUrl = string.Empty;
            MultiplayerEndpointUrl = string.Empty;
        }
    }
}
