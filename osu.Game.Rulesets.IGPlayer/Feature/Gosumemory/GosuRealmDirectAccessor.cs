using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Database;
using osu.Game.Models;
using Realms;
using File = System.IO.File;
using Path = System.IO.Path;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory;

public partial class GosuRealmDirectAccessor : CompositeDrawable
{
    private readonly RealmAccess realmAccess;
    private readonly Realm realm;

    public GosuRealmDirectAccessor(RealmAccess realmAccess)
    {
        this.realmAccess = realmAccess;
        this.realm = realm;
    }

    [Resolved]
    private Storage storage { get; set; } = null!;

    private string filesRoot()
    {
        return Path.Combine(storageRoot(), "files");
    }

    private string storageRoot()
    {
        return storage.GetFullPath(".");
    }

    public Task<string?> ExportSingleTask(BeatmapSetInfo setInfo, string targetFile, string? desti)
    {
        string? val = ExportFileSingle(setInfo, targetFile, desti);
        return Task.FromResult(val);
    }

    public string? ExportFileSingle(BeatmapSetInfo setInfo, string targetFile, string? desti)
    {
        try
        {
            string? path = exportFileSingle(setInfo, targetFile, desti);
            return path;
        }
        catch (Exception e)
        {
            Logging.LogError(e, "Error occurred while reading file.");
            Logger.Log(e.ToString());
            return null;
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="setInfo"></param>
    /// <param name="targetFile"></param>
    /// <param name="desti"></param>
    /// <returns>The final location of the exported file.</returns>
    private string? exportFileSingle(BeatmapSetInfo setInfo, string targetFile, string? desti)
    {
        RealmNamedFileUsage? realmNamedFileUsage = setInfo.Files.FirstOrDefault(fileUsage => fileUsage.Filename.Equals(targetFile));

        if (realmNamedFileUsage == null)
        {
            Logger.Log($"File not found for '{targetFile}'");
            return null;
        }

        string sourc = getRealmedFileName(realmNamedFileUsage.File.Hash);
        desti ??= Path.Combine(storageRoot(), realmNamedFileUsage.File.Hash.Substring(0, 8));

        try
        {
            string path = Path.Combine(filesRoot(), sourc);

            var parent = Directory.GetParent(path);

            if (parent == null)
            {
                Logger.Log($"{path} is filesystem root... It should'n be!");
                return null;
            }

            if (!parent.Exists)
                parent.Create();

            //Logger.Log($"Trying copy {path} to {desti}");

            if (File.Exists(desti))
            {
                //File.Delete(desti);
                //Logger.Log("File already exists! Skipping...");
                return desti;
            }

            File.Copy(path, desti);
        }
        catch (Exception e)
        {
            Logging.LogError(e, "Error occurred coping file to the destination!");
        }

        return desti;
    }

    private string getRealmedFileName(string hash)
    {
        string str = Path.Combine(hash[..1], hash[..2], hash);

        //Logger.Log("File is located at " + str);
        return str;
    }
}
