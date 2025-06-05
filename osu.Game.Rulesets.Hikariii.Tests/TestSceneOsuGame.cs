// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Game.Tests.Visual;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hikariii.Tests
{
    public partial class TestSceneOsuGame : OsuTestScene
    {
        [Resolved]
        private HikariiiTestBrowser testBrowser { get; set; } = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black,
                },
            };

            AddGame(new OsuGameFork
            {
                HoverChanged = onGameHoverChanged
            });

            AddToggleStep("切换Host光标", v =>
            {
                testBrowser.OsuHostCursorVisible.Value = v;
                testBrowser.SystemCursorVisible.Value = !v;
            });
        }

        private void onGameHoverChanged((OsuGame, bool) e)
        {
        }

        public partial class OsuGameFork : OsuGame
        {
            public Action<(OsuGame, bool)>? HoverChanged;

            protected override void OnHoverLost(HoverLostEvent e)
            {
                base.OnHoverLost(e);

                HoverChanged?.Invoke((this, false));
            }

            protected override bool OnHover(HoverEvent e)
            {
                HoverChanged?.Invoke((this, true));
                return base.OnHover(e);
            }
        }
    }
}
