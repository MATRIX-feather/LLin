using System;
using NetCoreServer;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;

namespace osu.Game.Rulesets.IGPlayer.Feature;

public partial class FeatureManager : CompositeDrawable
{
    public readonly BindableBool CanUseDBus = new(true);
    public readonly BindableBool CanUseGlazerMemory = new(true);

    public static FeatureManager? Instance { get; private set; }

    public FeatureManager()
    {
        if (Instance != null && Instance != this)
        {
            Logger.Log("Duplicate FeatureManager instance", level: LogLevel.Important);
            this.Expire();
            return;
        }

        Instance = this;

        // Check DBus
        if (!OperatingSystem.IsLinux())
        {
            CanUseDBus.Value = false;
        }
        else
        {
            try
            {
                string? tmdsDBusSystrmAddr = Tmds.DBus.Address.System;
                var mDbus = new M.DBus.ServiceUtils();
            }
            catch (Exception e)
            {
                if (e is not TypeLoadException) return;

                Logging.LogError(e, $"Unable to activate DBus integration: {e.Message}");
                CanUseDBus.Value = false;
            }
        }

        // Check GLazer
        try
        {
            var server = new HttpServer("127.0.0.1", 32763);
        }
        catch (Exception e)
        {
            if (e is not TypeLoadException) return;

            Logging.LogError(e, $"Unable to activate Gosu integration: {e.Message}");
            CanUseGlazerMemory.Value = false;
        }
    }
}
