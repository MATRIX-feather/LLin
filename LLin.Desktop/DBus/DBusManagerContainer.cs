using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LLin.Desktop.DBus.Tray;
using LLin.Game;
using LLin.Game.Configuration;
using LLin.Game.Graphics.Notifications;
using M.DBus;
using M.DBus.Services;
using M.DBus.Services.Kde;
using M.DBus.Services.Notifications;
using M.DBus.Tray;
using M.DBus.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Online.API;
using osu.Game.Overlays;
using osu.Game.Rulesets;
using osu.Game.Users;
using Tmds.DBus;

namespace LLin.Desktop.DBus
{
    public class DBusManagerContainer : Component, IHandleTrayManagement, IHandleSystemNotifications
    {
        public DBusManager DBusManager;

        public Action<SimpleNotification> NotificationAction { get; set; }
        private readonly Bindable<bool> controlSource;

        private readonly Bindable<UserActivity> bindableActivity = new Bindable<UserActivity>();
        private readonly MprisPlayerService mprisService = new MprisPlayerService();

        private readonly KdeStatusTrayService kdeTrayService = new KdeStatusTrayService();
        private readonly CanonicalTrayService canonicalTrayService = new CanonicalTrayService();
        private SDL2DesktopWindow sdl2DesktopWindow => (SDL2DesktopWindow)host.Window;

        private BeatmapInfoDBusService beatmapService;
        private AudioInfoDBusService audioservice;
        private UserInfoDBusService userInfoService;

        [Resolved]
        private IBindable<WorkingBeatmap> beatmap { get; set; }

        [Resolved]
        private IBindable<RulesetInfo> ruleset { get; set; }

        [Resolved]
        private MusicController musicController { get; set; }

        [Resolved]
        private GameHost host { get; set; }

        [Resolved]
        private LLinGame game { get; set; }

        [Resolved]
        private LargeTextureStore textureStore { get; set; }

        [Resolved]
        private MConfigManager config { get; set; }

        public DBusManagerContainer(bool autoStart = false, Bindable<bool> controlSource = null)
        {
            if (autoStart && controlSource != null)
                this.controlSource = controlSource;
            else if (controlSource == null && autoStart) throw new InvalidOperationException("???????????????????????????????????????null?");

            DBusManager = new DBusManager(false, this, this);
        }

        #region Disposal

        protected override void Dispose(bool isDisposing)
        {
            DBusManager.Dispose();
            base.Dispose(isDisposing);
        }

        #endregion

        protected override void LoadComplete()
        {
            controlSource?.BindValueChanged(onControlSourceChanged, true);
            beatmap.DisabledChanged += b => mprisService.BeatmapDisabled = b;
            base.LoadComplete();
        }

        protected override void Update()
        {
            base.Update();
            mprisService.TrackRunning = musicController.CurrentTrack.IsRunning;
        }

        private Bindable<bool> enableTray;
        private Bindable<bool> enableSystemNotifications;

        [BackgroundDependencyLoader]
        private void load(IAPIProvider api, Storage storage)
        {
            DBusManager.RegisterNewObjects(new IDBusObject[]
            {
                beatmapService = new BeatmapInfoDBusService(),
                audioservice = new AudioInfoDBusService(),
                userInfoService = new UserInfoDBusService()
            });

            DBusManager.GreetService.AllowPost = config.GetBindable<bool>(MSetting.DBusAllowPost);
            DBusManager.GreetService.OnMessageRecive = onMessageRevicedFromDBus;

            void onDBusConnected()
            {
                DBusManager.RegisterNewObject(mprisService,
                    "org.mpris.MediaPlayer2.mfosu");

                canonicalTrayService.AddEntryRange(new[]
                {
                    new SimpleEntry
                    {
                        Label = "LLin",
                        Enabled = false,
                        IconData = textureStore.GetStream("avatarlogo")?.ToByteArray()
                                   ?? SimpleEntry.EmptyPngBytes
                    },
                    new SimpleEntry
                    {
                        Label = "??????/????????????",
                        OnActive = () =>
                        {
                            sdl2DesktopWindow.Visible = !sdl2DesktopWindow.Visible;
                        },
                        IconName = "window-pop-out"
                    },
                    new SimpleEntry
                    {
                        Label = "??????",
                        OnActive = exitGame,
                        IconName = "application-exit"
                    },
                    new SeparatorEntry()
                });

                enableTray.BindValueChanged(onEnableTrayChanged, true);
                enableSystemNotifications.BindValueChanged(onEnableNotificationsChanged, true);

                DBusManager.OnConnected -= onDBusConnected;
            }

            enableTray = config.GetBindable<bool>(MSetting.EnableTray);
            enableSystemNotifications = config.GetBindable<bool>(MSetting.EnableSystemNotifications);

            DBusManager.OnConnected += onDBusConnected;

            api.LocalUser.BindValueChanged(onUserChanged, true);
            beatmap.BindValueChanged(onBeatmapChanged, true);
            ruleset.BindValueChanged(v => userInfoService.SetProperty(nameof(UserMetadataProperties.CurrentRuleset), v.NewValue?.Name ?? "???"), true);
            bindableActivity.BindValueChanged(v => userInfoService.SetProperty(nameof(UserMetadataProperties.Activity), v.NewValue?.Status ?? "??????"), true);

            mprisService.Storage = storage;
            beatmapService.Storage = storage;

            mprisService.UseAvatarLogoAsDefault = config.GetBindable<bool>(MSetting.MprisUseAvatarlogoAsCover);

            //??????Schedule????????????????????????Update?????????
            mprisService.Next += () => musicController.NextTrack(); //NextTrack???PreviousTrack?????????Schedule???
            mprisService.Previous += () => musicController.PreviousTrack();
            mprisService.Play += () => Schedule(() => musicController.Play(requestedByUser: true));
            mprisService.Pause += () => Schedule(() => musicController.Stop(true));
            mprisService.Quit += exitGame;
            mprisService.Seek += t => Schedule(() => musicController.SeekTo(t));
            mprisService.Stop += () => Schedule(() => musicController.Stop(true));
            mprisService.PlayPause += () => Schedule(() => musicController.TogglePause());
            //mprisService.OpenUri += s => Schedule(() => game.HandleLink(s));
            mprisService.WindowRaise += raiseWindow;

            kdeTrayService.WindowRaise += raiseWindow;
        }

        private void onEnableTrayChanged(ValueChangedEvent<bool> v)
        {
            if (v.NewValue)
            {
                DBusManager.RegisterNewObject(canonicalTrayService,
                    "io.matrix_feather.dbus.menu");

                DBusManager.RegisterNewObject(kdeTrayService,
                    "org.kde.StatusNotifierItem.mfosu");

                Task.Run(ConnectToWatcher);
            }
            else
            {
                DBusManager.UnRegisterObject(kdeTrayService);
                DBusManager.UnRegisterObject(canonicalTrayService);
            }
        }

        private void raiseWindow()
        {
            if (!sdl2DesktopWindow.Visible) sdl2DesktopWindow.Visible = true;

            Schedule(sdl2DesktopWindow.Raise);
        }

        private void exitGame() => Schedule(game.Exit);

        private void onMessageRevicedFromDBus(string message)
        {
            NotificationAction.Invoke(new SimpleNotification
            {
                Text = "??????????????????DBus?????????: \n" + message
            });

            Logger.Log($"??????????????????DBus?????????: {message}");
        }

        private void onUserChanged(ValueChangedEvent<User> v)
        {
            bindableActivity.UnbindBindings();
            bindableActivity.BindTo(v.NewValue.Activity);
            userInfoService.User = v.NewValue;
        }

        private void onBeatmapChanged(ValueChangedEvent<WorkingBeatmap> v)
        {
            beatmapService.Beatmap = v.NewValue;
            mprisService.Beatmap = v.NewValue;
            audioservice.Beatmap = v.NewValue;
        }

        private void onControlSourceChanged(ValueChangedEvent<bool> v)
        {
            if (v.NewValue)
                DBusManager.Connect();
            else
                DBusManager.Disconnect();
        }

        #region ??????

        private IStatusNotifierWatcher trayWatcher;

        public async Task<bool> ConnectToWatcher()
        {
            try
            {
                trayWatcher = DBusManager.GetDBusObject<IStatusNotifierWatcher>(new ObjectPath("/StatusNotifierWatcher"), "org.kde.StatusNotifierWatcher");

                await trayWatcher.RegisterStatusNotifierItemAsync("org.kde.StatusNotifierItem.mfosu").ConfigureAwait(false);
            }
            catch (Exception e)
            {
                trayWatcher = null;
                Logger.Error(e, "??????????????? org.kde.StatusNotifierWatcher, ?????????????????????");
                return false;
            }

            return true;
        }

        public void AddEntry(SimpleEntry entry)
        {
            canonicalTrayService.AddEntryToMenu(entry);
        }

        public void RemoveEntry(SimpleEntry entry)
        {
            canonicalTrayService.RemoveEntryFromMenu(entry);
        }

        #endregion

        #region ??????

        private void onEnableNotificationsChanged(ValueChangedEvent<bool> v)
        {
            if (v.NewValue)
            {
                connectToNotifications();
            }
            else
            {
                systemNotification = null;
            }
        }

        private INotifications systemNotification;

        private bool notificationWatched;
        private readonly Dictionary<uint, SystemNotification> notifications = new Dictionary<uint, SystemNotification>();

        private bool connectToNotifications()
        {
            try
            {
                var path = new ObjectPath("/org/freedesktop/Notifications");
                systemNotification = DBusManager.GetDBusObject<INotifications>(path, path.ToServiceName());

                if (!notificationWatched)
                {
                    //bug: ???gnome???????????????????????????
                    systemNotification.WatchActionInvokedAsync(onActionInvoked);
                    systemNotification.WatchNotificationClosedAsync(onNotificationClosed);
                    notificationWatched = true;
                }
            }
            catch (Exception e)
            {
                systemNotification = null;
                notificationWatched = false;
                Logger.Error(e, "??????????????? org.freedesktop.Notifications, ?????????????????????");
                return false;
            }

            return true;
        }

        private void onNotificationClosed((uint id, uint reason) singal)
        {
            SystemNotification notification;

            if (notifications.TryGetValue(singal.id, out notification))
            {
                notification.OnClosed?.Invoke(singal.reason.ToCloseReason());
                notifications.Remove(singal.id);
            }
        }

        private void onActionInvoked((uint id, string actionKey) obj)
        {
            SystemNotification notification;

            if (notifications.TryGetValue(obj.id, out notification))
            {
                notification.Actions.FirstOrDefault(a => a.Id == obj.actionKey)?.OnInvoked?.Invoke();
                notification.OnClosed?.Invoke(CloseReason.ActionInvoked);

                notifications.Remove(obj.id);
                Task.Run(async () => await CloseNotificationAsync(obj.id).ConfigureAwait(false));
            }
        }

        public async Task<uint> PostAsync(SystemNotification notification)
        {
            try
            {
                if (systemNotification != null)
                {
                    var target = notification.ToDBusObject();

                    var result = await systemNotification.NotifyAsync(target.appName,
                        target.replacesID,
                        target.appIcon,
                        target.title,
                        target.description,
                        target.actions,
                        target.hints,
                        target.displayTime).ConfigureAwait(false);

                    notifications[result] = notification;
                    return result;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "????????????????????????????????????");
            }

            return 0;
        }

        public async Task<bool> CloseNotificationAsync(uint id)
        {
            if (systemNotification != null)
            {
                await systemNotification.CloseNotificationAsync(id).ConfigureAwait(false);
                return true;
            }

            return false;
        }

        public async Task<string[]> GetCapabilitiesAsync()
        {
            if (systemNotification != null)
            {
                return await systemNotification.GetCapabilitiesAsync().ConfigureAwait(false);
            }

            return Array.Empty<string>();
        }

        private readonly (string name, string vendor, string version, string specVersion) defaultServerInfo = ("mfosu", "mfosu", "0", "0");

        public async Task<(string name, string vendor, string version, string specVersion)> GetServerInformationAsync()
        {
            if (systemNotification != null)
            {
                return await systemNotification.GetServerInformationAsync().ConfigureAwait(false);
            }

            return defaultServerInfo;
        }

        #endregion
    }
}
