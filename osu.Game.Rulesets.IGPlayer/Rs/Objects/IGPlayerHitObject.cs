// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osuTK;

namespace osu.Game.Rulesets.IGPlayer.Rs.Objects
{
    public class IGPlayerHitObject : HitObject, IHasPosition
    {
        public override Judgement CreateJudgement() => new Judgement();

        public float X { get; set; }
        public float Y { get; set; }
        public Vector2 Position { get; set; }
    }
}
