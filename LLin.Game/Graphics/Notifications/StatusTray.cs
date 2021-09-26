using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Localisation;
using osuTK;

namespace LLin.Game.Graphics.Notifications
{
    public class NotificationTray : FillFlowContainer
    {
        public NotificationTray()
        {
            Padding = new MarginPadding(25);
            Width = 400;
            AutoSizeAxes = Axes.Y;
            AutoSizeDuration = 300;
            AutoSizeEasing = Easing.OutQuint;
            Spacing = new Vector2(5);
            Direction = FillDirection.Vertical;
            Masking = true;
        }

        public void Post(SimpleNotification notification)
        {
            Add(notification);

            Show();
        }

        public void Post(LocalisableString text)
        {
            Post(new SimpleNotification
            {
                Text = text
            });
        }

        public override void Show()
        {
            this.MoveToY(0, 300, Easing.OutQuint)
                .FadeIn(300, Easing.OutQuint);
        }

        public override void Hide()
        {
            this.MoveToY(40, 300, Easing.OutQuint)
                .FadeOut(300, Easing.OutQuint);
        }
    }
}
