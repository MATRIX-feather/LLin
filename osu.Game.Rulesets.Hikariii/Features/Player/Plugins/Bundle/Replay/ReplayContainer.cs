using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Audio;
using osu.Game.Beatmaps;
using osu.Game.Overlays;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI;
using osu.Game.Scoring;
using osu.Game.Screens.Play;
using osu.Game.Skinning;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.Replay;

public partial class ReplayContainer : Container, ISamplePlaybackDisabler
{
    private readonly Score score;
    private readonly IImplementLLin llin;
    private DrawableRuleset drawableRuleset = null!;
    private Container playingContainer = null!;
    private ScoreProcessor scoreProcessor;

    [Resolved]
    private MusicController musicController { get; set; } = null!;

    public ReplayContainer(Score score, IImplementLLin llin)
    {
        this.score = score;
        this.llin = llin;
    }

    private DependencyContainer dependencies;

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    [BackgroundDependencyLoader]
    private void load(BeatmapManager beatmapManager)
    {
        Alpha = 0.01f;

        var beatmapInfo = score.ScoreInfo.BeatmapInfo;
        var workingBeatmap = beatmapManager.GetWorkingBeatmap(beatmapInfo);
        var playableBeatmap = workingBeatmap.GetPlayableBeatmap(score.ScoreInfo.Ruleset);

        var rulesetInstance = score.ScoreInfo.Ruleset.CreateInstance();
        this.drawableRuleset = rulesetInstance.CreateDrawableRulesetWith(playableBeatmap, score.ScoreInfo.Mods);
        drawableRuleset.Clock = llin.AudioClock;

        drawableRuleset.OnLoadComplete += _ =>
        {
            // Without calling `ApplyToPlayer`, OsuModRelax would crash the game due to their `pressHandler` is null where it should not.
            // Since ModRelax doesn't implement IApplicableToPlayer, and Hikariii itself doesn't have dependency to osu ruleset (Also not worth adding one just because this imo)
            // The best way is to perform like how Player does.
            var fakePlayer = new ReplayPlayer(score);
            foreach (var mod in drawableRuleset.Mods.OfType<IApplicableToPlayer>())
                mod.ApplyToPlayer(fakePlayer);
        };

        playingContainer = new RulesetSkinProvidingContainer(rulesetInstance, playableBeatmap, workingBeatmap.Skin)
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            RelativeSizeAxes = Axes.Both,
            Child = drawableRuleset
        };

        this.scoreProcessor = rulesetInstance.CreateScoreProcessor();
        scoreProcessor.Mods.Value = score.ScoreInfo.Mods;
        scoreProcessor.ApplyBeatmap(playableBeatmap);
        scoreProcessor.Clock = llin.AudioClock;
        scoreProcessor.HasCompleted.BindValueChanged(v =>
        {
            playingContainer.FadeTo(v.NewValue ? 0.01f : 1, 300, Easing.OutQuint)
                            .ScaleTo(v.NewValue ? 1.4f : 1f, 300, Easing.OutQuint);
        }, true);

        Add(scoreProcessor);

        dependencies.CacheAs(scoreProcessor);

        LoadComponent(playingContainer);
        Add(playingContainer);

        drawableRuleset.NewResult += r =>
        {
            //Logging.Log("NewResult " + r);
            scoreProcessor.ApplyResult(r);
        };

        drawableRuleset.RevertResult += r =>
        {
            scoreProcessor.RevertResult(r);
        };

        drawableRuleset.FrameStableClock.IsCatchingUp.BindValueChanged(v => this.updateSampleDisablingState());
        drawableRuleset.SetReplayScore(score);
    }

    private bool isPlaying;

    protected override void Update()
    {
        // Currently I don't know how to listen for whether the current audio track is playing, so let's just do this for now...
        bool playing = musicController.IsPlaying;

        if (playing != isPlaying)
        {
            isPlaying = playing;
            updateSampleDisablingState();
        }

        base.Update();
    }

    private void updateSampleDisablingState()
    {
        bool sampleDisabled = drawableRuleset.FrameStableClock.IsCatchingUp.Value || !isPlaying;
        samplePlaybackDisabled.Value = sampleDisabled;

        playingContainer.ScaleTo(sampleDisabled ? 0.9f : 1f, 300, Easing.OutQuint)
                        .FadeColour(sampleDisabled ? Color4.DimGray : Color4.White, 300, Easing.OutQuint);
    }

    public override void Show()
    {
        this.FadeIn(300, Easing.OutQuint);
        base.Show();
    }

    public override void Hide()
    {
        this.FadeOut(300, Easing.OutQuint);
        base.Hide();
    }

    private readonly BindableBool samplePlaybackDisabled = new BindableBool();
    public IBindable<bool> SamplePlaybackDisabled => samplePlaybackDisabled;
}
