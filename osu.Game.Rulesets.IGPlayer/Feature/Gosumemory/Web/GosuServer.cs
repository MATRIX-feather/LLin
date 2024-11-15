using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using NetCoreServer;
using osu.Framework.Platform;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Web;

public partial class GosuServer : WsServer
{
    public GosuServer(IPAddress address, int port)
        : base(address, port)
    {
    }

    public readonly AssetManager AssetManager = new AssetManager(new List<Storage>());

    private Storage staticsStorage;

    public void SetStorage(Storage statics, Storage caches)
    {
        this.staticsStorage = statics;

        AssetManager.SetStorages([statics, caches]);
    }

    public Storage? GetStorage()
    {
        return this.staticsStorage;
    }

    protected override TcpSession CreateSession() { return new GosuSession(this); }

    protected override void OnError(SocketError error)
    {
        Logging.Log($"WebSocket server caught an error with code {error}");
    }

    public void AddCustomHandler(string path, string urlPath, FileCache.InsertHandler handler)
    {
        TimeSpan timeout = TimeSpan.FromMilliseconds(100);
        this.Cache.InsertPath(path, urlPath, "*.*", timeout, handler);
    }

    public byte[]? FindStaticOrAsset(string path)
    {
        return this.AssetManager.FindAsset(path);
    }

    public new void AddStaticContent(string path, string prefix = "/", string filter = "*.*", TimeSpan? timeout = null)
    {
        throw new Exception("Deprecated operation");
    }
}
