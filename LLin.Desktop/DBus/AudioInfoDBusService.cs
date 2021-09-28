using System.Threading.Tasks;
using osu.Game.Beatmaps;
using Tmds.DBus;

namespace LLin.Desktop.DBus
{
    [DBusInterface("io.matrix_feather.mfosu.Audio")]
    public interface IAudioInfoDBusService : IDBusObject
    {
        Task<double> GetTrackLengthAsync();
        Task<double> GetTrackProgressAsync();
    }

    public class AudioInfoDBusService : IAudioInfoDBusService
    {
        public static readonly ObjectPath PATH = new("/io/matrix_feather/mfosu/Audio");

        public WorkingBeatmap Beatmap { get; set; }
        public ObjectPath ObjectPath => PATH;

        public Task<double> GetTrackLengthAsync()
        {
            return Task.FromResult(Beatmap?.Track.Length ?? 0d);
        }

        public Task<double> GetTrackProgressAsync()
        {
            return Task.FromResult(Beatmap?.Track.CurrentTime ?? 0d);
        }
    }
}
