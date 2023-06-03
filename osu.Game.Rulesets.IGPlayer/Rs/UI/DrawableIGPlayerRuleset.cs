// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Input;
using osu.Game.Beatmaps;
using osu.Game.Input.Handlers;
using osu.Game.Replays;
using osu.Game.Rulesets.IGPlayer.Rs.Objects;
using osu.Game.Rulesets.IGPlayer.Rs.Objects.Drawables;
using osu.Game.Rulesets.IGPlayer.Rs.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.IGPlayer.Rs.UI
{
    [Cached]
    public partial class DrawableIGPlayerRuleset : DrawableRuleset<IGPlayerHitObject>
    {
        public DrawableIGPlayerRuleset(IGPlayerRuleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods = null)
            : base(ruleset, beatmap, mods)
        {
        }

        protected override Playfield CreatePlayfield() => new IGPlayerPlayfield();

        protected override ReplayInputHandler CreateReplayInputHandler(Replay replay) => new IGPlayerFramedReplayInputHandler(replay);

        public override DrawableHitObject<IGPlayerHitObject> CreateDrawableRepresentation(IGPlayerHitObject h) => new DrawableIGPlayerHitObject(h);

        protected override PassThroughInputManager CreateInputManager() => new IGPlayerInputManager(Ruleset?.RulesetInfo);
    }
}
