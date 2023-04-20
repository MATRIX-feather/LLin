#nullable disable

using System;
using osu.Framework.Graphics;
using osu.Game.Overlays.Settings;
using osu.Game.Screens.Select;

namespace osu.Game.Rulesets.IGPlayer.Player.Screens
{
    public partial class MvisBeatmapDetailArea : BeatmapDetailArea
    {
        public Action SelectCurrentAction;

        protected override BeatmapDetailAreaTabItem[] CreateTabItems() => new BeatmapDetailAreaTabItem[]
        {
            new VoidTabItem(),
        };

        public MvisBeatmapDetailArea()
        {
            Add(
                new SettingsButton
                {
                    Text = "选择该谱面",
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Action = () => SelectCurrentAction?.Invoke()
                }
            );
        }

        private class VoidTabItem : BeatmapDetailAreaTabItem
        {
            public override string Name => "";
        }
    }
}
