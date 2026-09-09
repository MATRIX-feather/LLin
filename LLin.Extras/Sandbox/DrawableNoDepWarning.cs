using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins;
using osuTK.Graphics;

namespace LLin.Extras.Sandbox;

public partial class DrawableNoDepWarning(string name) : DrawableHikariiiPlugin
{
    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Add(new OsuSpriteText
        {
            Text = $"{name} 未安装",
            Colour = Color4.Red,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Font = OsuFont.GetFont(size: 24)
        });
    }
}
