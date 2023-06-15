using Newtonsoft.Json;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Data.Gameplay
{
    public struct KeyOverlayData
    {
        [JsonProperty("k1")]
        public KeyOverlayButton Keyboard1;

        [JsonProperty("k2")]
        public KeyOverlayButton Keyboard2;

        [JsonProperty("m1")]
        public KeyOverlayButton Mouse1;

        [JsonProperty("m2")]
        public KeyOverlayButton Mouse2;

        public void Reset()
        {
            Keyboard1.IsPressed = Keyboard2.IsPressed = Mouse1.IsPressed = Mouse2.IsPressed = false;
            Keyboard1.TotalPresses = Keyboard2.TotalPresses = Mouse1.TotalPresses = Mouse2.TotalPresses = 0;
        }
    }

    public struct KeyOverlayButton
    {
        [JsonProperty("isPressed")]
        public bool IsPressed;

        [JsonProperty("count")]
        public int TotalPresses;
    }
}
