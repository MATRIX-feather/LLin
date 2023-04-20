// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Rulesets.Replays;
using osuTK;

namespace osu.Game.Rulesets.IGPlayer.Replays
{
    public class IGPlayerReplayFrame : ReplayFrame
    {
        public List<IGPlayerAction> Actions = new List<IGPlayerAction>();
        public Vector2 Position;

        public IGPlayerReplayFrame(IGPlayerAction? button = null)
        {
            if (button.HasValue)
                Actions.Add(button.Value);
        }
    }
}
