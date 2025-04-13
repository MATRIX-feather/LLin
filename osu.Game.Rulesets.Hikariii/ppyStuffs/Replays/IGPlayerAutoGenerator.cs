// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hikariii.ppyStuffs.Objects;
using osu.Game.Rulesets.Replays;

namespace osu.Game.Rulesets.Hikariii.ppyStuffs.Replays
{
    public class IGPlayerAutoGenerator : AutoGenerator<IGPlayerReplayFrame>
    {
        public new Beatmap<HikariiiPlayerHitObject> Beatmap => (Beatmap<HikariiiPlayerHitObject>)base.Beatmap;

        public IGPlayerAutoGenerator(IBeatmap beatmap)
            : base(beatmap)
        {
        }

        protected override void GenerateFrames()
        {
            Frames.Add(new IGPlayerReplayFrame());

            foreach (HikariiiPlayerHitObject hitObject in Beatmap.HitObjects)
            {
                Frames.Add(new IGPlayerReplayFrame
                {
                    Time = hitObject.StartTime,
                    Position = hitObject.Position,
                    // todo: add required inputs and extra frames.
                });
            }
        }
    }
}
