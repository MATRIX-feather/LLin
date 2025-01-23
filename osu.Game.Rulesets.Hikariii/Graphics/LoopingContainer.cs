using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace osu.Game.Rulesets.Hikariii.Graphics;

public partial class LoopingContainer : Container
{
    public override void Add(Drawable drawable)
    {
        if (drawable.RelativePositionAxes == Axes.X)
            throw new Exception("RelativePositionAxes for drawables must not be X");

        base.Add(drawable);
        loopingDrawables.Add(drawable);
    }

    //private OsuSpriteText text;

    [BackgroundDependencyLoader]
    private void load()
    {
        //AddInternal(text = new OsuSpriteText());
    }

    public LoopingContainer()
    {
    }

    private List<Drawable> loopingDrawables = new List<Drawable>();

    private float totalLength = 0;

    private void sortChildren()
    {
        totalLength = 0;
        float offset = (float)Clock.CurrentTime / 5000f;
        int index = -1;

        foreach (var drawable in Children)
        {
            index++;

            totalLength += drawable.DrawWidth;

            drawable.X = (this.DrawWidth * offset + drawable.Width * index);
        }

        foreach (var drawable in Children)
        {
            drawable.X %= totalLength;
        }
    }

    protected override void LoadComplete()
    {
        sortChildren();
        base.LoadComplete();
    }

    protected override void Update()
    {
        double elapsedTime = Clock.ElapsedFrameTime;

        bool towardsRight = true;

        float offset = (float)Clock.CurrentTime / 5000f;
        int index = -1;

        // 研究了三个小时最后的结果 :)
        foreach (var drawable in Children)
        {
            index++;
            float diff = 100 * (float)elapsedTime / 1000f;

            if (towardsRight)
            {
                drawable.X += diff;

                if (drawable.X >= this.DrawWidth)
                    drawable.X -= totalLength;
            }
            else
            {
                drawable.X -= diff;

                if (drawable.X <= -drawable.DrawWidth)
                    drawable.X += totalLength;
            }
        }

        //text.Text = $"totLength: {totalLength},  Time :: {Clock.CurrentTime}";

        base.Update();
    }
}
