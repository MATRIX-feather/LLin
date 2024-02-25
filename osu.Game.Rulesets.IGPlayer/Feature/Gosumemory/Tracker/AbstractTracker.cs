using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Timing;
using osu.Game.Graphics.Sprites;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Tracker;

public partial class AbstractTracker : CompositeDrawable
{
    protected TrackerHub Hub { get; private set; }

    public AbstractTracker(TrackerHub hub)
    {
        this.Hub = hub;
        AlwaysPresent = true;

#if DEBUG

        InternalChild = new OsuSpriteText
        {
            Text = $"{this}",
            Margin = new MarginPadding(30)
        };

#endif
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        this.Clock = new FramedClock(null, false);
    }

    public virtual void UpdateValues()
    {
    }
}
