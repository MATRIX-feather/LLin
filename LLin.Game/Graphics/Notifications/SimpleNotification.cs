using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;
using osuTK.Graphics;

namespace LLin.Game.Graphics.Notifications
{
    public class SimpleNotification : CompositeDrawable
    {
        public LocalisableString Text { get; set; }
        public NotificationLevel Level = NotificationLevel.Normal;

        public SimpleNotification()
        {
            Masking = true;
            CornerRadius = 7.5f;
            AutoSizeAxes = Axes.Y;
            RelativeSizeAxes = Axes.X;
            EdgeEffect = new EdgeEffectParameters
            {
                Type = EdgeEffectType.Shadow,
                Radius = 1.5f,
                Colour = Color4.Black.Opacity(0.6f),
                Offset = new Vector2(0, 1.5f)
            };
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = getBackgroundColorForLevel(Level)
                },
                new MTextFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Colour = Level == NotificationLevel.Error ? Color4.White : Color4.Black,
                    Margin = new MarginPadding(15),
                    Text = Text.ToString()
                }
            };
        }

        private Color4 getBackgroundColorForLevel(NotificationLevel level)
        {
            switch (level)
            {
                case NotificationLevel.Error:
                    return Color4.Red;

                case NotificationLevel.Warning:
                    return Color4.Gold;

                default:
                    return Color4.White;
            }
        }

        protected override void LoadComplete()
        {
            this.Delay(2000).Then().FadeOut(300, Easing.OutQuint);
            base.LoadComplete();
        }

        protected override bool OnClick(ClickEvent e)
        {
            Expire();
            return base.OnClick(e);
        }
    }

    public enum NotificationLevel
    {
        Normal,
        Warning,
        Error
    }
}
