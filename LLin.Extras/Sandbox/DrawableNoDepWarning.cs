using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins;
using osuTK.Graphics;

namespace LLin.Extras.Sandbox;

public partial class DrawableNoDepWarning(LocalisableString name, string releasePage) : DrawableHikariiiPlugin
{
    [BackgroundDependencyLoader]
    private void load()
    {
        LinkFlowContainer textFlow;
        AutoSizeAxes = Axes.Both;
        Masking = true;
        CornerRadius = 8;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        Children =
        [
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Color4.Black.Opacity(0.5f)
            },
            textFlow = new LinkFlowContainer(st => st.Font = OsuFont.GetFont(size: 18))
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                AutoSizeAxes = Axes.Both,
                Margin = new MarginPadding(8),
            }
        ];

        textFlow.AddParagraph("Sandbox2Panel 依赖 ");
        textFlow.AddText(name);
        textFlow.AddText("，但其并未安装!");

        textFlow.AddParagraph("您可前往");
        textFlow.AddLink("此处", releasePage);
        textFlow.AddText("下载安装必要的文件，然后重启游戏以使其生效");
    }
}
