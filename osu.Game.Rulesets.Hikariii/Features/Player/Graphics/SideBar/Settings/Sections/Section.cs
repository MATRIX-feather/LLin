using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osuTK;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SideBar.Settings.Sections
{
    public abstract partial class Section : CompositeDrawable, ISidebarContent
    {
        public string Title
        {
            get => title.Text.ToString();
            set => title.Text = value;
        }

        public IconUsage Icon { get; set; }

        private readonly OsuSpriteText title = new OsuSpriteText
        {
            Font = OsuFont.GetFont(size: 30),
            Anchor = Anchor.TopRight,
            Origin = Anchor.TopRight
        };

        protected Section()
        {
            AutoSizeAxes = Axes.Y;
            RelativeSizeAxes = Axes.X;
            Anchor = Origin = Anchor.TopRight;
            Padding = new MarginPadding(10);

            InternalChildren = new Drawable[]
            {
                title,
                FillFlow = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Y,
                    RelativeSizeAxes = Axes.X,
                    Spacing = new Vector2(5),
                    Margin = new MarginPadding { Top = 40 },
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.TopRight
                }
            };

            Anchor = Origin = Anchor.TopRight;
        }

        protected void FadeoutThen(double fadeOutDuration, Action action)
        {
            this.FadeTo(0.01f, fadeOutDuration, Easing.OutQuint)
                .Then()
                .Schedule(action.Invoke)
                .Then()
                .FadeIn(200, Easing.OutQuint);
        }

        protected readonly FillFlowContainer FillFlow;

        protected void AddRange(Drawable[] drawables)
        {
            foreach (var drawable in drawables)
            {
                Add(drawable);
            }
        }

        protected void Add(Drawable drawable) => FillFlow.Add(drawable);
    }
}
