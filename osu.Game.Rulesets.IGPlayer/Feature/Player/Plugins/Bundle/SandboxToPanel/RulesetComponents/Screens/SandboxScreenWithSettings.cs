using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.SandboxToPanel.RulesetComponents.UI.Settings;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.SandboxToPanel.RulesetComponents.Screens
{
    public abstract partial class SandboxScreenWithSettings : SandboxScreen
    {
        private readonly SandboxSettings settings;
        protected readonly BindableBool SettingsVisible = new BindableBool();

        public SandboxScreenWithSettings()
        {
            AddRangeInternal(new Drawable[]
            {
                CreateBackground(),
                new GridContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    RowDimensions = new[]
                    {
                        new Dimension()
                    },
                    ColumnDimensions = new[]
                    {
                        new Dimension(),
                        new Dimension(GridSizeMode.AutoSize)
                    },
                    Content = new[]
                    {
                        new Drawable[]
                        {
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Child = CreateContent()
                            },
                            settings = new SandboxSettings
                            {
                                Sections = CreateSettingsSections()
                            }
                        }
                    }
                }
            });

            SettingsVisible.BindTo(settings.IsVisible);
        }

        protected abstract Drawable CreateContent();

        protected new abstract Drawable CreateBackground();

        protected abstract SandboxSettingsSection[] CreateSettingsSections();

        protected override bool OnMouseMove(MouseMoveEvent e)
        {
            base.OnMouseMove(e);

            if (SettingsVisible.Value)
                return false;

            var cursorPosition = ToLocalSpace(e.CurrentState.Mouse.Position);

            if (cursorPosition.X > DrawWidth - 5)
            {
                SettingsVisible.Value = true;
                return true;
            }

            return false;
        }

        protected override bool OnClick(ClickEvent e)
        {
            if (!SettingsVisible.Value)
                return false;

            SettingsVisible.Value = false;
            return true;
        }

        protected override void Dispose(bool isDisposing)
        {
            SettingsVisible.UnbindFrom(settings.IsVisible);
            base.Dispose(isDisposing);
        }
    }
}
