#nullable disable
using System;
using System.Threading.Tasks;
using osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Mpris.Properties;
using Tmds.DBus;

namespace osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Mpris.Interfaces;

[DBusInterface("org.mpris.MediaPlayer2")]
public interface IMediaPlayer2 : IDBusObject
{
    Task RaiseAsync();
    Task QuitAsync();
    Task<object> GetAsync(string prop);
    Task<MediaPlayer2Properties> GetAllAsync();
    Task SetAsync(string prop, object val);
    Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler);
}
