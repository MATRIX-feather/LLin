// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Database;
using osu.Game.Online.API;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.IGPlayer.Helper.Handler;
using osu.Game.Rulesets.IGPlayer.Rs.Beatmaps;
using osu.Game.Rulesets.IGPlayer.Rs.Mods;
using osu.Game.Rulesets.IGPlayer.Rs.UI;
using osu.Game.Rulesets.IGPlayer.Settings.Mf;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.IGPlayer
{
    public partial class IGPlayerRuleset : Ruleset
    {
        public override string Description => "Hikariii (下载加速&音乐播放器&Gosu支持)";

        public override string ShortName => "igplayerruleset";

        public override string PlayingVerb => "正在听歌";

        public override RulesetSettingsSubsection? CreateSettings()
        {
            return new MfMainSection(this);
        }

        public override DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap, IReadOnlyList<Mod>? mods) =>
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

        public override IEnumerable<KeyBinding> GetDefaultKeyBindings(int variant = 0) => new KeyBinding[]
        {
            new KeyBinding(InputKey.Left, HikariiiAction.MusicPrev),
            new KeyBinding(InputKey.Right, HikariiiAction.MusicNext),
            new KeyBinding(InputKey.Space, HikariiiAction.TogglePause),

            new KeyBinding(InputKey.Enter, HikariiiAction.OpenInSongSelect),
            new KeyBinding(InputKey.Tab, HikariiiAction.ToggleOverlayLock),
            new KeyBinding(InputKey.L, HikariiiAction.TrackLoop),

            new KeyBinding(InputKey.Comma, HikariiiAction.TogglePluginPage),
            new KeyBinding(InputKey.Slash, HikariiiAction.TogglePlayList),

            new KeyBinding(InputKey.H, HikariiiAction.LockOverlays)
        };

        public override Drawable CreateIcon() => new Icon()
        {
        };

        public partial class Icon : CompositeDrawable
        {
            public Icon()
            {
                RelativeSizeAxes = Axes.Both;
                Masking = false;
            }

            [BackgroundDependencyLoader(permitNulls: true)]
            private void load(OsuGame game, Storage storage, IModelImporter<BeatmapSetInfo> beatmapImporter, IAPIProvider api)
            {
                try
                {
                    Size = new Vector2(20);
                    InternalChildren = new Drawable[]
                    {
                        new Circle
                        {
                            Size = new Vector2(1),
                            Colour = Color4.White,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            RelativeSizeAxes = Axes.Both,
                            Child = new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = Color4.Black.Opacity(0)
                            },
                            BorderColour = Color4.White,
                            BorderThickness = 45
                        },
                        new SpriteIcon
                        {
                            Size = new Vector2(0.6f),
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Icon = FontAwesome.Solid.YinYang,
                            RelativeSizeAxes = Axes.Both,
                        }
                    };

                    Logging.Log("Injecting dependencies...");
                    Logging.Log($"Deps: Game = '{game}' :: Storage = '{storage}' :: Importer = '{beatmapImporter}' :: IAPIProvider = '{api}'");

                    if (OsuGameHandler.InjectDependencies(storage, game, this.Scheduler)) return;

                    Logging.Log("Inject failed!", level: LogLevel.Error);
                    return;
                }
                catch (Exception e)
                {
                    Logging.LogError(e, "??");
                }
            }
        }

        // Leave this line intact. It will bake the correct version into the ruleset on each build/release.
        public override string RulesetAPIVersionSupported => CURRENT_RULESET_API_VERSION;
    }
}
