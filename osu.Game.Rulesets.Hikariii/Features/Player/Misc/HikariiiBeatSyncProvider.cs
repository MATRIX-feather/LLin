using System;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Timing;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Beatmaps.Timing;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Misc;

public partial class HikariiiBeatSyncProvider : CompositeComponent, IBeatSyncProvider
{
    private OsuSpriteText ampText;
    private OsuSpriteText cpText;
    private OsuSpriteText cText;
    private Sync beatContainer;

    [Resolved]
    private Bindable<WorkingBeatmap> beatmap { get; set; } = null!;

    [Resolved]
    private IImplementLLin llin { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren =
        [
            ampText = new OsuSpriteText(),
            cpText = new OsuSpriteText
            {
                Y = 20
            },
            cText = new OsuSpriteText
            {
                Y = 40,
            },
            beatContainer = new Sync
            {
                Divisor = 2,
                NewBeat = str =>
                {
                    oldText?.FadeOut().Then().Expire();
                    oldText = null;

                    var txt = new OsuSpriteText
                    {
                        Y = 60,
                        Text = str
                    };

                    oldText = txt;

                    this.AddInternal(txt);
                    txt.FadeOut(300).Then().Expire();
                }
            }
        ];
    }

    private OsuSpriteText? oldText;

    private partial class Sync : BeatSyncedContainer
    {
        public Action<string>? NewBeat;

        public bool SyncedWithBeat => IsBeatSyncedWithTrack;

        private int? firstBeat;
        private bool canPlay;

        protected override void OnNewBeat(int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes)
        {
            int beatsPerBar = timingPoint.TimeSignature.Numerator;
            int segmentLength = beatsPerBar * Divisor * 4;

            if (!IsBeatSyncedWithTrack)
            {
                firstBeat = null;

                NewBeat?.Invoke($"NewBeat Not Synced! Synced? {SyncedWithBeat} :: beatIndex {beatIndex} :: firstBeat {(firstBeat.HasValue ? firstBeat : "NoValue")}");
                return;
            }

            if (!firstBeat.HasValue || beatIndex < firstBeat)
                // decide on a good starting beat index if once has not yet been decided.
                firstBeat = beatIndex < 0 ? 0 : (beatIndex / segmentLength + 1) * segmentLength;

            canPlay = beatIndex >= firstBeat;

            NewBeat?.Invoke($"NewBeat! Synced? {SyncedWithBeat}"
                            + $":: beatIndex {beatIndex}"
                            + $"::firstBeat {(!firstBeat.HasValue ? "NoValue" : firstBeat)}"
                            + $":: canPlay {canPlay}"
                            + $":: playBeatFor({(beatIndex % 4)}, {timingPoint.TimeSignature.Numerator}) => {playBeatFor(beatIndex % 4, timingPoint.TimeSignature)}");
            base.OnNewBeat(beatIndex, timingPoint, effectPoint, amplitudes);
        }

        private string playBeatFor(int beatIndex, TimeSignature signature)
        {
            if (beatIndex == 0)
                return "FinishSample";

            switch (signature.Numerator)
            {
                case 3:
                    switch (beatIndex % 6)
                    {
                        case 0:
                            return "KickSample";

                        case 3:
                            return "ClapSample";

                        default:
                            return "HatSample";
                    }

                    break;

                case 4:
                    switch (beatIndex % 4)
                    {
                        case 0:
                            return "KickSample";
                            break;

                        case 2:
                            return "ClapSample";
                            break;

                        default:
                            return "HatSample";
                            break;
                    }

                    break;
            }

            return "NoSample!";
        }
    }

    protected override void Update()
    {
        base.Update();

        cpText.Text = $"ControlPoints => {CPInfo}";
        ampText.Text = $"Amplitudes => {Amplitudes}";
        cText.Text = $"Clock => {AudioClock}";
    }

    public ControlPointInfo? CPInfo => beatmap.Value.BeatmapLoaded ? beatmap.Value.Beatmap.ControlPointInfo : null;
    public ChannelAmplitudes Amplitudes => llin.CurrentTrack.CurrentAmplitudes;
    public IClock AudioClock => llin.AudioClock;

    ChannelAmplitudes IHasAmplitudes.CurrentAmplitudes => Amplitudes;
    ControlPointInfo? IBeatSyncProvider.ControlPoints => CPInfo;
    IClock IBeatSyncProvider.Clock => AudioClock;
}
