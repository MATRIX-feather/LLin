using Tmds.DBus;

#nullable enable

namespace M.DBus
{
    public interface IMDBusObject : IDBusObject
    {
        /// <summary>
        /// Service name
        /// </summary>
        public string? CustomRegisterName { get; }

        public bool IsService { get; }
    }
}
