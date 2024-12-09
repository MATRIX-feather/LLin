using System.Linq;
using System.Reflection;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Localisation;
using osu.Framework.Logging;
using osu.Game.Overlays;
using osu.Game.Overlays.Settings;

namespace osu.Game.Rulesets.IGPlayer.Settings.Mf
{
    public sealed partial class MfMainSection : RulesetSettingsSubsection
    {
        protected override LocalisableString Header { get; } = "Hikariii";

        private readonly SubPanel subPanel = new();

        private readonly SettingsButton settingsButton;

        public MfMainSection(Ruleset ruleset)
            : base(ruleset)
        {
            var button = new SettingsButton();

            button.Text = "打开设置面板";
            button.Action = () => subPanel.ToggleVisibility();
            Add(button);

            settingsButton = button;
        }

        [BackgroundDependencyLoader]
        private void load(SettingsOverlay settingsOverlay)
        {
            var targetMethod = settingsOverlay.GetType()
                                              .GetRuntimeMethods()
                                              .FirstOrDefault(method => method.Name == "createSubPanel")
                                              ?.MakeGenericMethod(typeof(SubPanel));

            if (targetMethod == null)
            {
                Logging.Log($"未能找到对应的方法, 无法添加Hikariii设置到界面中", level: LogLevel.Important);
                settingsButton.Enabled.Value = false;
                return;
            }

            object? targetContainer = settingsOverlay.GetType()
                                                     .GetRuntimeFields()
                                                     .FirstOrDefault(field => field.Name == "ContentContainer")
                                                     ?.GetValue(settingsOverlay);

            if (targetContainer is not Container<Drawable> contentContainer)
            {
                Logging.Log($"未能找到对应的字段, 无法添加Hikariii设置到界面中", level: LogLevel.Important);
                settingsButton.Enabled.Value = false;
                return;
            }

            targetMethod.Invoke(settingsOverlay, [this.subPanel]);
            this.Schedule(() => contentContainer.Add(subPanel));
        }
    }
}
