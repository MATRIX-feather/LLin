using System;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.CloudMusic.Misc;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.CloudMusic.Helper.LyricProperties.Processors;

public class AuthorProcessor : IPropertyProcessor
{
    public bool Process(string propertyInput, Lyric lyricRef)
    {
        if (!propertyInput.StartsWith("by:", StringComparison.Ordinal))
            return false;

        // todo: Implement this

        return true;
    }
}
