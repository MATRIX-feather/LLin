using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using LLin.Game.Screens.Mvis.SideBar.Tabs;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;

namespace LLin.Game.Screens.Mvis.SideBar
{
    internal class Sidebar : VisibilityContainer
    {
        [Resolved]
        private CustomColourProvider colourProvider { get; set; }

        [CanBeNull]
        [Resolved(CanBeNull = true)]
        private MvisScreen mvisScreen { get; set; }

        public readonly List<ISidebarContent> Components = new List<ISidebarContent>();
        public TabControl Header;
        private const float duration = 400;
        private TabControlItem prevTab;

        public Bindable<Drawable> CurrentDisplay = new Bindable<Drawable>();

        private readonly Container<Drawable> contentContainer;
        protected override Container<Drawable> Content => contentContainer;

        private Sample sampleToggle;
        private Sample samplePopIn;
        private Sample samplePopOut;

        private bool startFromHiddenState;
        private bool isFirstHide = true;
        private readonly Box bgBox;

        public Sidebar()
        {
            RelativeSizeAxes = Axes.Both;

            InternalChildren = new Drawable[]
            {
                bgBox = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black.Opacity(0.6f)
                },
                contentContainer = new Container
                {
                    Name = "Content",
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                }
            };
        }

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            sampleToggle = audio.Samples.Get("UI/overlay-pop-in");
            samplePopIn = audio.Samples.Get("UI/overlay-pop-in");
            samplePopOut = audio.Samples.Get("UI/overlay-pop-out");
        }

        protected override void LoadComplete()
        {
            CurrentDisplay.BindValueChanged(onCurrentDisplayChanged);
            base.LoadComplete();
        }

        private void onCurrentDisplayChanged(ValueChangedEvent<Drawable> v)
        {
            if (!(v.NewValue is ISidebarContent sc)) return;

            prevTab?.MakeInActive();

            foreach (var t in Header.Tabs)
            {
                if (t.Value == sc)
                {
                    t.MakeActive();
                    prevTab = t;
                    break;
                }
            }

            if (!startFromHiddenState)
                sampleToggle?.Play();
        }

        protected override void UpdateAfterChildren()
        {
            contentContainer.Padding = new MarginPadding
            {
                Right = Header.GetRightUnavaliableSpace(),
                Left = Header.GetLeftUnavaliableSpace(),
                Top = Header.GetTopUnavaliableSpace(),
                Bottom = mvisScreen?.BottombarHeight ?? 0
            };

            base.UpdateAfterChildren();
        }

        public void ShowComponent(Drawable d, bool allowHide = false)
        {
            if (!(d is ISidebarContent c))
                throw new InvalidOperationException($"{d}??????{typeof(ISidebarContent)}");

            if (!Components.Contains(c))
                throw new InvalidOperationException($"????????????????????????{c}");

            startFromHiddenState = State.Value == Visibility.Hidden;

            if (CurrentDisplay.Value == d)
            {
                //????????????????????????????????????????????????????????????
                if (State.Value == Visibility.Visible)
                {
                    if (allowHide) Hide();

                    return;
                }
                else
                    prevTab?.MakeActive();
            }

            Show();

            var resizeDuration = startFromHiddenState ? 0 : duration;

            var lastDisplay = CurrentDisplay.Value;
            lastDisplay?.FadeOut(resizeDuration / 2, Easing.OutQuint)
                       .OnComplete(_ => contentContainer.Remove(lastDisplay));

            if (!contentContainer.Contains(d))
                contentContainer.Add(d);

            //??????????????????????????????????????????????????????????????????FadeTo(0.01f)?????????????????????OnComplete
            d.FadeTo(0.01f).FadeTo(0).Then()
             .Delay(resizeDuration / 2).FadeIn(resizeDuration / 2);

            CurrentDisplay.Value = d;
        }

        private void addDrawableToList(Drawable d)
        {
            if (d is ISidebarContent s)
            {
                d.Alpha = 0;
                Components.Add(s);
                Header.Tabs.Add(new TabControlItem(s)
                {
                    Action = () => ShowComponent(d, true)
                });
            }
        }

        public override void Add(Drawable drawable) => addDrawableToList(drawable);

        public override void Clear(bool disposeChildren)
        {
            Header.Tabs.Clear(disposeChildren);
            contentContainer.Clear(disposeChildren);
        }

        public override bool Remove(Drawable drawable)
        {
            if (drawable is ISidebarContent sc)
            {
                foreach (var t in Header.Tabs)
                {
                    if (t.Value == sc)
                    {
                        Header.Tabs.Remove(t);
                        drawable.Expire();
                        return true;
                    }
                }
            }

            return base.Remove(drawable);
        }

        protected override void PopOut()
        {
            if (!isFirstHide)
                samplePopOut?.Play();
            else
                isFirstHide = false;

            Header.SidebarActive = false;
            Header.Hide();
            bgBox.FadeOut(duration, Easing.OutQuint);

            contentContainer.FadeOut(duration, Easing.OutQuint)
                            .MoveToY(70, duration, Easing.OutQuint);

            prevTab?.MakeInActive();
        }

        protected override void PopIn()
        {
            samplePopIn?.Play();

            Header.SidebarActive = true;
            Header.Show();
            bgBox.FadeIn(duration, Easing.OutQuint);

            contentContainer.FadeIn(duration, Easing.OutQuint)
                            .MoveToY(0, duration, Easing.OutQuint);
        }
    }
}
