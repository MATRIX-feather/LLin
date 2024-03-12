using System;
using NetCoreServer;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Game.Configuration;
using osu.Game.Rulesets.IGPlayer.Settings.AccelUtils;

namespace osu.Game.Rulesets.IGPlayer.Feature;

public partial class FeatureManager : CompositeDrawable
{
    public readonly BindableBool CanUseDBus = new(false);
    public readonly BindableBool CanUseGlazerMemory = new(false);

    public static FeatureManager? Instance { get; private set; }

    [BackgroundDependencyLoader]
    private void load(OsuConfigManager osuConfig)
    {
        AccelExtensionsUtil.SetOsuConfigManager(osuConfig);
    }

    public FeatureManager()
    {
    }
}
