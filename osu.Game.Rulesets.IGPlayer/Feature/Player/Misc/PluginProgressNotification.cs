#nullable disable

using System;
using osu.Game.Overlays.Notifications;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Misc
{
    public partial class PluginProgressNotification : ProgressNotification
    {
        public Action OnComplete { get; set; }

        public override void Close(bool runFlingAnimation)
        {
            OnComplete?.Invoke();
            base.Close(runFlingAnimation);
        }
    }
}
