namespace osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Notifications;

public interface INotificationImpl
{
    void EnableDoNotDisturb(string reason);
    void DisableDoNotDisturb();
}
