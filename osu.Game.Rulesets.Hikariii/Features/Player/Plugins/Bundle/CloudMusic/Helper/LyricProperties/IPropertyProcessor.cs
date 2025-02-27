using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.CloudMusic.Misc;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.CloudMusic.Helper.LyricProperties;

public interface IPropertyProcessor
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="propertyInput"></param>
    /// <param name="lyricRef"></param>
    /// <returns>此Processor是否成功处理了输入的属性和Lyric</returns>
    public bool Process(string propertyInput, Lyric lyricRef);
}
