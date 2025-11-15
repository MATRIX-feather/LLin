using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Screens;
using osu.Game.Rulesets.Hikariii.Features.Configuration;
using osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Notifications;
using osu.Game.Screens.Play;
using osu.Game.Screens.Ranking;

namespace osu.Game.Rulesets.Hikariii.Features.ListenerLoader.Handlers.ScreenHandlers;

public partial class GamePlayerHandler : AbstractScreenHandler
{
    [Resolved(canBeNull: false)]
    private SystemNotificationIntegration? notifications { get; set; }

    [Resolved]
    private MConfigManager config { get; set; } = null!;

    private readonly BindableBool enabled = new();

    [BackgroundDependencyLoader]
    private void load()
    {
        config.BindWith(MSetting.EnableOSDoNotDisturbWhenPlaying, enabled);
        enabled.BindValueChanged(onToggle);
    }

    private void onToggle(ValueChangedEvent<bool> v)
    {
        if (!v.NewValue)
            notifications?.SetDoNotDisturb(false, "Disabled by user");
        else
            updateStatus();
    }

    private void updateStatus()
    {
        if (shouldEnableDoNotDisturb)
            notifications?.SetDoNotDisturb(true, "Playing game");
        else
            notifications?.SetDoNotDisturb(false, "");
    }

    private bool shouldEnableDoNotDisturb = false;

    public override void Handle(IScreen prev, IScreen next)
    {
        // prev is ResultsScreen and next is SubmittingPlayer -> We're probably exiting from results screen?
        if (prev is ResultsScreen && next is SubmittingPlayer)
            return;

        shouldEnableDoNotDisturb = next is SubmittingPlayer;

        if (enabled.Value)
            updateStatus();
    }
}
