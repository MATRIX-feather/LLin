using System.Collections.Generic;
using System.IO;
using osu.Framework.Platform;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Web;

public partial class AssetManager
{
    private readonly List<Storage> storages = new List<Storage>();

    public AssetManager(List<Storage> storages)
    {
        SetStorages(storages);
    }

    public void SetStorages(List<Storage> newList)
    {
        storages.Clear();
        storages.AddRange(newList);
    }

    public byte[] FindAsset(string relativePath)
    {
        foreach (var storage in storages)
        {
            if (!storage.Exists(relativePath)) continue;

            string fullPath = storage.GetFullPath(relativePath);
            return File.ReadAllBytes(fullPath);
        }

        return [];
    }
}
