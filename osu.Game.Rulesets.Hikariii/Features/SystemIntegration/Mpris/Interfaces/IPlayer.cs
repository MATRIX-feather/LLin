#nullable disable
using System;
using System.Threading.Tasks;
using osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Mpris.Properties;
using Tmds.DBus;

namespace osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Mpris.Interfaces;

[DBusInterface("org.mpris.MediaPlayer2.Player")]
public interface IPlayerController : IDBusObject
{
    Task NextAsync();
    Task PreviousAsync();
    Task PauseAsync();
    Task PlayPauseAsync();
    Task StopAsync();
    Task PlayAsync();
    Task SeekAsync(long offset);
    Task SetPositionAsync(ObjectPath trackId, long position);
    Task OpenUriAsync(string uri);
    Task<IDisposable> WatchSeekedAsync(Action<long> handler, Action<Exception> onError = null);
    Task<object> GetAsync(string prop);
    Task<PlayerProperties> GetAllAsync();
    Task SetAsync(string prop, object val);
    Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler);
}
