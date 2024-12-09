using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Game.Configuration;
using osu.Game.Rulesets.IGPlayer.Settings.AccelUtils;

namespace osu.Game.Rulesets.IGPlayer.Feature;

public partial class FeatureManager : CompositeDrawable
{
    public readonly BindableBool CanUseDBus = new(true);

    public static FeatureManager? Instance { get; private set; }

    [BackgroundDependencyLoader]
    private void load(OsuConfigManager osuConfig)
    {
        AccelExtensionsUtil.SetOsuConfigManager(osuConfig);
    }

    public FeatureManager()
    {
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
                // 尝试访问Tmds.DBus和M.DBus中的值，如果访问成功则代表安装了DBus集成
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
    }
}
