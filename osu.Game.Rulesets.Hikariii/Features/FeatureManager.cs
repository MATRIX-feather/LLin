using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Game.Configuration;
using osu.Game.Rulesets.Hikariii.Features.DownloadAccel.AccelUtils;
using Tmds.DBus.Protocol;

namespace osu.Game.Rulesets.Hikariii.Features;

public partial class FeatureManager : CompositeDrawable
{
    public readonly BindableBool CanUseDBus = new(false);

    public static FeatureManager? Instance { get; private set; }

    [BackgroundDependencyLoader]
    private void load(OsuConfigManager osuConfig)
    {
        AccelExtensionsUtil.SetOsuConfigManager(osuConfig);

        try
        {
            tryDbus();
        }
        catch (Exception e)
        {
            Logging.LogError(e, "Failed call tryDbus");
        }
    }

    public FeatureManager()
    {
        Instance = this;
    }

    private void tryDbus()
    {
        // Check DBus
        if (!OperatingSystem.IsLinux())
        {
            CanUseDBus.Value = false;
        }
        else
        {
            try
            {
                // 尝试访问Tmds.DBus和M.DBus中的值，如果访问成功则代表安装了DBus集成
                string? tmdsDBusSystrmAddr = Address.System;
                var mDbus = new M.DBus.ServiceUtils();
            }
            catch (Exception e)
            {
                Logging.LogError(e, $"Unable to activate DBus integration: {e.Message}");
                CanUseDBus.Value = false;
            }
        }
    }
}
