using System.Collections.Generic;
using System.ComponentModel;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.Helper;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.Misc;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.LyricProvider;

public abstract partial class LyricProvider : CompositeDrawable
{
    protected LyricProvider()
    {
    }

    [Resolved]
    private Storage osuStorage { get; set; }

    protected Storage Storage => osuStorage;

    public abstract void Lookup(SearchOption beatmap);

    public virtual void Save(IWorkingBeatmap beatmap, List<Lyric> lyrics)
    {
    }

    public enum SearchState
    {
        [Description("未找到歌曲或信息不匹配")]
        Fail,

        [Description("搜索中")]
        Searching,

        [Description("模糊搜索中")]
        FuzzySearching,

        [Description("已就绪")]
        Success
    }

    public readonly Bindable<SearchState> State = new Bindable<SearchState>();

    protected void SetState(SearchState newState)
    {
        State.Value = newState;
    }
}
