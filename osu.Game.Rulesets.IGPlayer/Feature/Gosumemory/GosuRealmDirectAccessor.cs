using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Database;
using osu.Game.Models;
using File = System.IO.File;
using Path = System.IO.Path;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory;

public partial class GosuRealmDirectAccessor : CompositeDrawable
{
    private readonly RealmAccess realmAccess;

    public GosuRealmDirectAccessor(RealmAccess realmAccess)
    {
        this.realmAccess = realmAccess;
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

    public enum OperationIfExists
    {
        OVERWRITE,
        KEEP
    }

    public Task<string?> ExportSingleTask(BeatmapSetInfo setInfo, string targetFile, string? desti, OperationIfExists operationIfExists = OperationIfExists.KEEP)
    {
        string? val = ExportFileSingle(setInfo, targetFile, desti, operationIfExists);
        return Task.FromResult(val);
    }

    public string? ExportFileSingle(BeatmapSetInfo setInfo, string targetFile, string? desti, OperationIfExists operationIfExists = OperationIfExists.KEEP)
    {
        try
        {
            string? path = exportFileSingle(setInfo, targetFile, desti, operationIfExists);
            return path;
        }
        catch (Exception e)
        {
            Logging.LogError(e, "Error occurred while reading file.");
            Logging.Log(e.ToString());
            return null;
        }
    }

    public string? GetRealmedPath(BeatmapSetInfo setInfo, string targetFile)
    {
        RealmNamedFileUsage? realmNamedFileUsage = setInfo.Files.FirstOrDefault(fileUsage => fileUsage.Filename.Equals(targetFile));

        if (realmNamedFileUsage == null)
            return null;

        return getRealmedFileName(realmNamedFileUsage.File.Hash);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="setInfo"></param>
    /// <param name="targetFile"></param>
    /// <param name="desti"></param>
    /// <returns>The final location of the exported file.</returns>
    private string? exportFileSingle(BeatmapSetInfo setInfo, string targetFile, string? desti, OperationIfExists operationIfExists = OperationIfExists.KEEP)
    {
        if (string.IsNullOrEmpty(targetFile)) return null;

        RealmNamedFileUsage? realmNamedFileUsage = setInfo.Files.FirstOrDefault(fileUsage => fileUsage.Filename.Equals(targetFile));

        if (realmNamedFileUsage == null)
        {
            Logging.Log($"File not found for '{targetFile}'");
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
                Logging.Log($"{path} is filesystem root... It shouldn't be!");
                return null;
            }

            if (!parent.Exists)
                parent.Create();

            //Logging.Log($"Trying copy {path} to {desti}");

            if (File.Exists(desti))
            {
                switch (operationIfExists)
                {
                    case OperationIfExists.OVERWRITE:
                        File.Delete(desti);
                        break;

                    case OperationIfExists.KEEP:
                        return desti;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(operationIfExists), operationIfExists, null);
                }

                //Logging.Log("File already exists! Skipping...");
                //return desti;
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

        //Logging.Log("File is located at " + str);
        return str;
    }
}
