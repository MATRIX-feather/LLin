using System;
using osu.Framework.Graphics;
using osu.Game.Graphics.Containers;

namespace osu.Game.Rulesets.Hikariii.Features;

public partial class AboutHikariiiOverlay : OsuFocusedOverlayContainer
{
    public override bool AcceptsFocus => true;
    public override bool RequestsFocus => true;

    public float FadeOutWaitDuration = 0;
    public Action? OnFadeOut;
    public Action? OnFadeIn;

    protected override void PopIn()
    {
        OnFadeIn?.Invoke();
        this.FadeIn();
    }

    protected override void PopOut()
    {
        OnFadeOut?.Invoke();
        this.Delay(FadeOutWaitDuration).FadeOut();
    }
}
