using System;
using LLin.Game.Screens.Mvis.Misc;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
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

        private readonly Container bgContent = new Container
        {
            RelativeSizeAxes = Axes.Both
        };

        [Resolved]
        private IBindable<WorkingBeatmap> b { get; set; }

        private readonly BeatmapCover bg;

        public Bindable<ScreenStatus> CurrentStatus = new Bindable<ScreenStatus>
        {
            Default = ScreenStatus.Scaled,
            Value = ScreenStatus.Scaled
        };

        public ScreenContainer()
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
                bgContent,
                content,
                new BasicButton
                {
                    Text = "切换隐藏",
                    Action = () => CurrentStatus.Value = (CurrentStatus.Value == ScreenStatus.Hidden ? ScreenStatus.Scaled : ScreenStatus.Hidden),
                    Size = new Vector2(50, 50)
                }
            };
            bgContent.AddRange(new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4Extensions.FromHex("#333")
                },
                bg,
                new Toolbar.Toolbar()
            });

            ((ClickableScreenContentContainer)content).ClickEvent += () => CurrentStatus.Value = ScreenStatus.Display;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
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
                bgContent.Show();
                content.CornerRadius = 15;
                content.TweenEdgeEffectTo(shadowEffect);
            }

            switch (v.NewValue)
            {
                case ScreenStatus.Display:
                    content.ScaleTo(1, 500, Easing.OutQuint).MoveToY(0, 500, Easing.OutQuint)
                           .OnComplete(_ => bgContent.Hide());
                    break;

                case ScreenStatus.Scaled:
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
