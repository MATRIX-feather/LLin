// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.IGPlayer.Beatmaps;
using osu.Game.Rulesets.IGPlayer.Mods;
using osu.Game.Rulesets.IGPlayer.Player;
using osu.Game.Rulesets.IGPlayer.UI;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.IGPlayer
{
    public partial class IGPlayerRuleset : Ruleset
    {
        public override string Description => "a very igplayerruleset ruleset";

        public override DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap, IReadOnlyList<Mod> mods = null) =>
            new DrawableIGPlayerRuleset(this, beatmap, mods);

        public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap) =>
            new IGPlayerBeatmapConverter(beatmap, this);

        public override DifficultyCalculator CreateDifficultyCalculator(IWorkingBeatmap beatmap) =>
            new IGPlayerDifficultyCalculator(RulesetInfo, beatmap);

        public IGPlayerRuleset()
        {
            rsInfo = this.RulesetInfo;
        }

        private static RulesetInfo? rsInfo;

        public static RulesetInfo? GetRulesetInfo()
        {
            return rsInfo;
        }

        public override IEnumerable<Mod> GetModsFor(ModType type)
        {
            switch (type)
            {
                case ModType.Automation:
                    return new[] { new IGPlayerModAutoplay() };

                default:
                    return Array.Empty<Mod>();
            }
        }

        public override string ShortName => "igplayerruleset";

        public override IEnumerable<KeyBinding> GetDefaultKeyBindings(int variant = 0) => new KeyBinding[]
        {
            new KeyBinding(InputKey.Left, IGAction.MusicPrev),
            new KeyBinding(InputKey.Right, IGAction.MusicNext),
            new KeyBinding(InputKey.Space, IGAction.TogglePause),

            new KeyBinding(InputKey.Enter, IGAction.OpenInSongSelect),
            new KeyBinding(InputKey.Tab, IGAction.ToggleOverlayLock),
            new KeyBinding(InputKey.L, IGAction.TrackLoop),

            new KeyBinding(InputKey.Comma, IGAction.TogglePluginPage),
            new KeyBinding(InputKey.Slash, IGAction.TogglePlayList),

            new KeyBinding(InputKey.H, IGAction.LockOverlays),
            new KeyBinding(InputKey.Escape, IGAction.Back)
        };

        public override Drawable CreateIcon() => new Icon(ShortName[0])
        {
            RelativeSizeAxes = Axes.Both
        };

        public partial class Icon : CompositeDrawable
        {
            public Icon(char c)
            {
                InternalChildren = new Drawable[]
                {
                    new Circle
                    {
                        Size = new Vector2(20),
                        Colour = Color4.White,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    },
                    new SpriteText
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Text = c.ToString(),
                        Font = OsuFont.Default.With(size: 18)
                    }
                };
            }

            [BackgroundDependencyLoader]
            private void load(OsuGame game, Storage storage)
            {
                Logger.Log("[IGPlayer] Injecting dependencies...");

                if (!OsuGameInjector.InjectDependencies(storage, game, this.Scheduler))
                {
                    Logger.Log("[IGPlayer] Inject failed!");
                    return;
                }

                this.Schedule(() =>
                    game.Add(new GameScreenInjector()));
            }
        }

        // Leave this line intact. It will bake the correct version into the ruleset on each build/release.
        public override string RulesetAPIVersionSupported => CURRENT_RULESET_API_VERSION;
    }
}
