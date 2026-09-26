using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SettingsItems;
using osu.Game.Rulesets.Hikariii.Features.Player.Screens.LLin;
using osu.Game.Tests.Visual;
using osuTK;

namespace osu.Game.Rulesets.Hikariii.Tests.HikariiiPlayerv2;

public partial class TestInPlayerOption : OsuTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        List<SettingsEntry> options =
        [
            new BooleanSettingsEntry
            {
                Name = "Boolean toggle",
                Description = "Description",
                Bindable = new Bindable<bool>()
            }
        ];

        var fillFlow = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.Both,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(5)
        };

        Add(fillFlow);

        Dependencies.Cache(new CustomColourProvider());

        foreach (var settingsEntry in options)
        {
            fillFlow.Add(settingsEntry.ToLLinSettingsItemV2() ?? throw new Exception($"Option {settingsEntry} doesn't have drawable llin settings item."));
        }
    }
}
