using System;
using JetBrains.Annotations;
using LLin.Game.Screens.Mvis.Misc;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Beatmaps;
using osu.Game.Input.Bindings;
using osuTK;
using osuTK.Graphics;

namespace LLin.Game.Graphics.Containers
{
    public class ScreenContainer : Container, IKeyBindingHandler<GlobalAction>
    {
        protected override Container<Drawable> Content => content;

        private readonly Container content = new ClickableScreenContentContainer
        {
            Name = "屏幕容器",
            RelativeSizeAxes = Axes.Both,
            RelativePositionAxes = Axes.Both,
            Masking = true,
            Anchor = Anchor.BottomCentre,
            Origin = Anchor.BottomCentre,
            Scale = new Vector2(0.6f),
            Y = 1,
            Alpha = 0
        };

        internal readonly Container BackgroundContents = new Container
        {
            Name = "背景容器",
            RelativeSizeAxes = Axes.Both
        };

        internal readonly Container BackgroundOverlays = new Container
        {
            Name = "其他背景",
            RelativeSizeAxes = Axes.Both,
            Width = 0.9f,
            Height = 0.85f,
            Anchor = Anchor.BottomCentre,
            Origin = Anchor.BottomCentre
        };

        [Resolved]
        private IBindable<WorkingBeatmap> b { get; set; }

        private BeatmapCover bg;

        public Bindable<ScreenStatus> CurrentStatus = new Bindable<ScreenStatus>
        {
            Default = ScreenStatus.Scaled,
            Value = ScreenStatus.Scaled
        };

        private DependencyContainer dependencies;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        [BackgroundDependencyLoader]
        private void load()
        {
            bg = new BeatmapCover(null)
            {
                BackgroundBox = false,
                TimeBeforeWrapperLoad = 0,
                UseBufferedBackground = true,
                Colour = Color4Extensions.FromHex("#666")
            };

            InternalChildren = new Drawable[]
            {
                BackgroundContents,
                content
            };

            dependencies.Cache(this);

            BackgroundContents.AddRange(new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4Extensions.FromHex("#333")
                },
                bg,
                new Toolbar.Toolbar(),
                BackgroundOverlays
            });

            ((ClickableScreenContentContainer)content).ClickEvent += () => CurrentStatus.Value = ScreenStatus.Display;
            CurrentStatus.BindValueChanged(onStatusChanged, true);
            b.BindValueChanged(v => bg.UpdateBackground(v.NewValue));
        }

        private readonly EdgeEffectParameters shadowEffect = new EdgeEffectParameters
        {
            Type = EdgeEffectType.Shadow,
            Radius = 30,
            Colour = Color4.Black.Opacity(0.4f)
        };

        private readonly EdgeEffectParameters defaultEffect = new EdgeEffectParameters();

        private void onStatusChanged(ValueChangedEvent<ScreenStatus> v)
        {
            if (v.NewValue != ScreenStatus.Hidden)
                content.FadeIn(500, Easing.OutQuint);

            if (v.NewValue == ScreenStatus.Display)
            {
                content.CornerRadius = 0;
                content.TweenEdgeEffectTo(defaultEffect);
            }
            else
            {
                BackgroundContents.Show();
                content.CornerRadius = 15;
                content.TweenEdgeEffectTo(shadowEffect);
            }

            switch (v.NewValue)
            {
                case ScreenStatus.Display:
                    ShowBackgroundOverlay(null);
                    content.ScaleTo(1, 500, Easing.OutQuint).MoveToY(0, 500, Easing.OutQuint)
                           .OnComplete(_ => BackgroundContents.Hide());
                    break;

                case ScreenStatus.Scaled:
                    ShowBackgroundOverlay(null);
                    content.ScaleTo(0.9f, 500, Easing.OutQuint).MoveToY(0.1f, 500, Easing.OutQuint);
                    break;

                case ScreenStatus.Hidden:
                    content.ScaleTo(0.6f, 500, Easing.OutQuint).MoveToY(1, 500, Easing.OutQuint)
                           .FadeOut(500, Easing.OutQuint);
                    break;
            }
        }

        public bool OnPressed(KeyBindingPressEvent<GlobalAction> e)
        {
            switch (e.Action)
            {
                case GlobalAction.ToggleSettings:
                    CurrentStatus.Value = (CurrentStatus.Value == ScreenStatus.Display ? ScreenStatus.Scaled : ScreenStatus.Display);
                    break;

                case GlobalAction.Back:
                    CurrentStatus.Value = ScreenStatus.Display;
                    break;
            }

            return false;
        }

        public void OnReleased(KeyBindingReleaseEvent<GlobalAction> e)
        {
        }

        internal void OnGameExit(Action onComplete)
        {
            CurrentStatus.Value = ScreenStatus.Hidden;
            this.FadeOut(300, Easing.OutQuint).OnComplete(_ => onComplete?.Invoke());
        }

        [CanBeNull]
        private BackgroundOverlayContainer currentOverlay;

        private bool overlaysHidden;

        internal void ShowBackgroundOverlay(BackgroundOverlayContainer overlay)
        {
            currentOverlay?.MoveToY(1, 300, Easing.OutQuint)
                          .FadeOut(300, Easing.OutQuint)
                          .OnComplete(removeOrExpire);

            currentOverlay?.OnPopOut();

            if (overlay == null) return;

            if (currentOverlay == overlay && !overlaysHidden)
            {
                overlaysHidden = true;
                CurrentStatus.Value = ScreenStatus.Scaled;
                return;
            }

            if (!BackgroundOverlays.Contains(overlay))
            {
                overlay.Y = 1;
                overlay.Alpha = 0;

                BackgroundOverlays.Add(overlay);
            }

            CurrentStatus.Value = ScreenStatus.Hidden;

            overlaysHidden = false;
            overlay.OnPopIn();
            overlay.MoveToY(0, 300, Easing.OutQuint).FadeIn(300, Easing.OutQuint);
            currentOverlay = overlay;
        }

        private void removeOrExpire(BackgroundOverlayContainer container)
        {
            if (container.ExpireAfterPopOut) container.Expire();
            else BackgroundOverlays.Remove(container);

            currentOverlay = null;
        }

        private class ClickableScreenContentContainer : ClickableContainer
        {
            public event Action ClickEvent;

            protected override bool OnClick(ClickEvent e)
            {
                ClickEvent?.Invoke();

                return base.OnClick(e);
            }
        }
    }

    public enum ScreenStatus
    {
        Display,
        Scaled,
        Hidden
    }
}
