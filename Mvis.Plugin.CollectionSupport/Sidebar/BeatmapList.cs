using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Caching;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Beatmaps;
using osu.Game.Graphics.Containers;
using LLin.Game.Screens.Mvis;
using osuTK;

namespace Mvis.Plugin.CollectionSupport.Sidebar
{
    public class BeatmapList : CompositeDrawable
    {
        [Resolved]
        private BeatmapManager beatmaps { get; set; }

        [Resolved]
        private Bindable<WorkingBeatmap> working { get; set; }

        private readonly List<BeatmapSetInfo> beatmapSets;
        private readonly Cached scrollCache = new Cached();
        private BeatmapPiece currentPiece;
        private OsuScrollContainer beatmapScroll;
        private FillFlowContainer fillFlow;

        private bool firstScroll = true;

        public BindableBool IsCurrent = new BindableBool();

        public BeatmapList(List<BeatmapSetInfo> set)
        {
            RelativeSizeAxes = Axes.Both;
            Alpha = 0;

            beatmapSets = set;
        }

        [Resolved]
        private CustomColourProvider colourProvider { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = beatmapScroll = new OsuScrollContainer
            {
                RelativeSizeAxes = Axes.Both,
                RightMouseScrollbar = true,
                Child = fillFlow = new FillFlowContainer
                {
                    Padding = new MarginPadding { Horizontal = 35 },
                    Margin = new MarginPadding { Vertical = 30 },
                    Spacing = new Vector2(5),
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                }
            };

            addBeatmapSets();
            working.BindValueChanged(OnBeatmapChanged);
            IsCurrent.BindValueChanged(v =>
            {
                foreach (var d in fillFlow)
                {
                    if (d is BeatmapPiece piece)
                        piece.IsCurrent = v.NewValue;
                }

                currentPiece?.TriggerActiveChange();
            });
        }

        private Box createDefaultMaskBox()
        {
            var b = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = getMaskBoxColour()
            };

            return b;
        }

        private ColourInfo getMaskBoxColour()
        {
            return ColourInfo.GradientVertical(
                colourProvider.Dark6,
                colourProvider.Dark6.Opacity(0));
        }

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();

            if (!scrollCache.IsValid) scrollToCurrent();
        }

        private void OnBeatmapChanged(ValueChangedEvent<WorkingBeatmap> v)
        {
            currentPiece?.InActive();
            currentPiece = null;

            foreach (var d in fillFlow)
            {
                if (!(d is BeatmapPiece piece)
                    || piece.Beatmap.BeatmapSetInfo.Hash != v.NewValue.BeatmapSetInfo.Hash) continue;

                currentPiece = piece;
                piece.MakeActive();
                break;
            }

            scrollCache.Invalidate();
        }

        private void addBeatmapSets()
        {
            fillFlow.AddRange(beatmapSets.Select(s => new BeatmapPiece(beatmaps.GetWorkingBeatmap(s.Beatmaps.First()))));

            scrollCache.Invalidate();
        }

        private void scrollToCurrent()
        {
            if (currentPiece == null || !IsCurrent.Value)
            {
                firstScroll = false;
                scrollCache.Validate();
                return;
            }

            var index = fillFlow.IndexOf(currentPiece);

            //???????????????????????????????????????
            if (index == 0)
            {
                beatmapScroll.ScrollToStart(!firstScroll);
            }
            else
            {
                //?????????????????????30??????(?)???????????????????????????????????????????????????????????????????????????????????????????????????
                //??????????????????30?????????????????????????????????(index-1)??????beatmapPiece???????????????
                //?????????(index-1)??????????????????????????????????????????????????????????????????
                float distance = ((index - 1) * 85) + 30;

                //???????????????????????????beatmapFillFlow??????????????????????????????
                //n???piece, n-1?????????
                if (distance + beatmapScroll.DrawHeight > ((fillFlow?.Count * 85) - 5))
                    beatmapScroll.ScrollToEnd(!firstScroll);
                else
                    beatmapScroll.ScrollTo(distance, !firstScroll);
            }

            scrollCache.Validate();
            firstScroll = false;
        }

        public void ClearList() =>
            fillFlow.Clear();

        public override void Show()
        {
            this.FadeIn(250);

            //??????: ????????????????????????beatmapScroll?????????Update?????????????????????????????????
            working.TriggerChange();
        }

        public override void Hide()
        {
            this.FadeOut(250);
        }
    }
}
