using LLin.Desktop.DBus;
using LLin.Game;
using LLin.Game.Configuration;
using osu.Framework.Allocation;

namespace LLin.Desktop
{
    public class LLinGameDesktop : LLinGame
    {
        private DBusManagerContainer dBusManagerContainer;

        private DependencyContainer dependencies;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        [BackgroundDependencyLoader]
        private void load()
        {
            dBusManagerContainer = new DBusManagerContainer(
                true,
                MConfig.GetBindable<bool>(MSetting.DBusIntegration));

            dependencies.Cache(dBusManagerContainer.DBusManager);
            Add(dBusManagerContainer);
            dBusManagerContainer.NotificationAction += n => NotificationTray.Post(n);
        }
    }
}
