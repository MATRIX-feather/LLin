#nullable disable

using System;
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets.IGPlayer.Configuration;
using osu.Game.Screens.Select;
using osuTK;

namespace osu.Game.Rulesets.IGPlayer.Player.Screens.SongSelect
{
    public partial class MvisBeatmapDetailArea : BeatmapDetailArea
    {
        public Action<bool> SelectCurrentAction;

        protected override BeatmapDetailAreaTabItem[] CreateTabItems() => new BeatmapDetailAreaTabItem[]
        {
            new VoidTabItem(),
        };

        private class BSC : BeatSyncedContainer
        {
            protected override void OnNewBeat(int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes)
            {
                this.Child.ScaleTo(1.1f).ScaleTo(1, 1000, Easing.OutQuint);
                base.OnNewBeat(beatIndex, timingPoint, effectPoint, amplitudes);
            }
        }

        private OsuCheckbox checkbox;

        public readonly BindableBool StartFromZero = new BindableBool();

        [BackgroundDependencyLoader]
        private void load(MConfigManager config)
        {
            // AddButton
            Add(
                new OsuAnimatedButton
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Action = () => SelectCurrentAction?.Invoke(checkbox?.Current.Value ?? true),
                    RelativeSizeAxes = Axes.Both,
                    Size = new Vector2(0.7f),

                    Child = new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(25),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,

                        Children = new Drawable[]
                        {
                            new BSC
                            {
                                Size = new Vector2(60),
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Child = new SpriteIcon
                                {
                                    Icon = FontAwesome.Regular.ArrowAltCircleLeft,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    RelativeSizeAxes = Axes.Both
                                }
                            },
                            new OsuSpriteText
                            {
                                Text = "选择该谱面",
                                Font = OsuFont.GetFont(size: 28),
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre
                            }
                        }
                    }
                }
            ); // Look at this nesting, oh my!

            // AddOption
            Add(checkbox = new OsuCheckbox(false)
            {
                RelativeSizeAxes = Axes.None,
                Width = 125,

                LabelText = "从头开始",
                LabelPadding = new MarginPadding { Horizontal = 75 },
                Anchor = Anchor.BottomRight,
                Origin = Anchor.BottomRight,
                Margin = new MarginPadding { Vertical = 20, Horizontal = 10 }
            });

            checkbox.Current.BindTo(StartFromZero);
        }

        private class VoidTabItem : BeatmapDetailAreaTabItem
        {
            public override string Name => "选择谱面";
        }
    }
}
