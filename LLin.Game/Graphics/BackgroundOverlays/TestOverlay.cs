using System;
using System.Collections.Generic;
using LLin.Game.Graphics.BackgroundOverlays.Settings;
using osu.Framework;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;
using osu.Game.Graphics;
using osuTK.Graphics;

namespace LLin.Game.Graphics.BackgroundOverlays
{
    public class TestOverlay : BackgroundOverlayContainer
    {
        public TestOverlay()
        {
            RelativePositionAxes = Axes.Both;
            RelativeSizeAxes = Axes.Both;

            Masking = true;
        }

        protected override Container<Drawable> Content => content;

        private readonly Container content = new Container
        {
            RelativeSizeAxes = Axes.Both
        };

        private MTextFlowContainer tipText;

        [BackgroundDependencyLoader]
        private void load()
        {
            TestOverlayHeader header;
            InternalChildren = new Drawable[]
            {
                header = new TestOverlayHeader(),
                content
            };

            content.Padding = new MarginPadding { Top = header.Height };

            Add(tipText = new MTextFlowContainer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Colour = Color4.White.Opacity(0.8f),
                TextAnchor = Anchor.Centre,
                RelativeSizeAxes = Axes.Both
            });
        }

        public override bool ExpireAfterPopOut => false;

        public override void OnPopIn()
        {
            tipText.Clear();
            tipText.AddText(hints[RNG.Next(0, hints.Count - 1)], t => t.Font = OsuFont.GetFont(size: 50));
        }

        public override void OnPopOut()
        {
            tipText.Clear();
            tipText.AddText("∑(っ °Д °;)っ", t => t.Font = OsuFont.GetFont(size: 50));
        }

        private readonly List<string> hints = new List<string>
        {
            "owo",
            "o.o",
            "Segmentation fault (core dumped)",
            "Program received signal SIGTRAP",
            "$ apt moo",
            "您今天moo了吗?",
            "$ sudo pacman -Syu",
            ":: 正在获取软件包......",
            "今日无事可做",
            "$ sudo apt updgrade",
            $"[sudo] {Environment.UserName}的密码：",
            "升级了 0 个软件包， 新安装了 0 个软件包， 要卸载 0 个软件包， 有 0 个软件包未被升级。",
            $"I use {RuntimeInfo.OS} btw.",
            $"{Environment.UserName} is you"
        };
    }
}
