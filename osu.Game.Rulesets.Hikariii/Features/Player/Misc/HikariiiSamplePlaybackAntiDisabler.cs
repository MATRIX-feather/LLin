using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Game.Audio;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Misc;

public partial class HikariiiSamplePlaybackAntiDisabler : CompositeComponent, ISamplePlaybackDisabler
{
    public IBindable<bool> SamplePlayDisabled = new Bindable<bool>(false);

    IBindable<bool> ISamplePlaybackDisabler.SamplePlaybackDisabled => SamplePlayDisabled;
}
