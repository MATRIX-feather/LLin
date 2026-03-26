using System.ComponentModel;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Collection.Sorter;

public enum SortMethod
{
    [Description("低难度优先")]
    EasiestFirst,

    [Description("高难度优先")]
    MostDifficultFirst,

    [Description("随机")]
    Random
}
