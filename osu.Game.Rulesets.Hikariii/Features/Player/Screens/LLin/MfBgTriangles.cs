using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.Backgrounds;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces;
using osu.Game.Rulesets.Hikariii.ppyStuffs.osu;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Screens.LLin
{
    public partial class MBgTriangles : VisibilityContainer
    {
        private readonly BackgroundTriangles backgroundTriangle;
        public readonly Bindable<bool> UseV2 = new();
        protected override bool StartHidden => false;

        private static readonly Color4 default_triangles_color = OsuColour.Gray(0.2f);

        public MBgTriangles(float alpha = 0.65f, Color4? trianglesColor = null, float triangleScale = 2f, bool withBeat = false)
        {
            Alpha = alpha;
            Masking = true;
            RelativeSizeAxes = Axes.Both;
            Anchor = Anchor.BottomCentre;
            Origin = Anchor.BottomCentre;

            trianglesColor ??= default_triangles_color;
            Children =
            [
                backgroundTriangle = new BackgroundTriangles((Color4)trianglesColor, triangleScale)
                {
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    AllowBeatSync = withBeat,
                    UseV2 = { BindTarget = UseV2 }
                }
            ];
        }

        protected override void LoadComplete()
        {
            this.Show();
            base.LoadComplete();
        }

        private partial class BackgroundTriangles : Container
        {
            private Triangles? triangles;
            private TrianglesV2Copy? trianglesV2;
            public readonly float TriangleScale;

            public bool AllowBeatSync;

            private readonly Color4 triangleColor;

            public BindableBool UseV2 { get; } = new();

            public BackgroundTriangles(Color4 trianglesColor, float triangleScaleValue = 2f)
            {
                RelativeSizeAxes = Axes.Both;
                TriangleScale = triangleScaleValue;

                triangleColor = trianglesColor;
            }

            [BackgroundDependencyLoader]
            private void load(IImplementLLin? llin)
            {
                InternalChildren =
                [
                    trianglesV2 ??= new TrianglesV2Copy(seed: 727 + llin?.SessionMagicCode ?? 0)
                    {
                        Anchor = Anchor.BottomCentre,
                        Origin = Anchor.BottomCentre,
                        RelativeSizeAxes = Axes.Both,
                        Colour = triangleColor,
                        Alpha = 0,
                        SpawnRatio = 1,
                        ExtraScaleRange = (0.2f, 2f)
                    },
                    triangles ??= new TrianglesV1Wrapper(seed: 727 + llin?.SessionMagicCode ?? 0)
                    {
                        Anchor = Anchor.BottomCentre,
                        Origin = Anchor.BottomCentre,
                        RelativeSizeAxes = Axes.Both,
                        TriangleScale = TriangleScale,
                        Colour = triangleColor,
                        Alpha = 0
                    }
                ];

                UseV2.BindValueChanged(v => updateTriangle(v.NewValue), true);
            }

            private void updateTriangle(bool useV2)
            {
                if (useV2)
                {
                    trianglesV2.FadeIn(300, Easing.OutQuint);
                    triangles.FadeOut(300, Easing.OutQuint);
                }
                else
                {
                    trianglesV2.FadeOut(300, Easing.OutQuint);
                    triangles.FadeIn(300, Easing.OutQuint);
                }
            }

            [Resolved]
            private IBindable<WorkingBeatmap> b { get; set; } = null!;

            protected override void Update()
            {
                if (!AllowBeatSync) return;

                float[] amplitudes = b.Value?.Track.CurrentAmplitudes.FrequencyAmplitudes.ToArray() ?? new float[256];
                bool isKiai = b.Value?.Beatmap.ControlPointInfo.EffectPointAt(b.Value?.Track?.CurrentTime ?? 0).KiaiMode ?? false;
                float totalSum = 0.1f + amplitudes.Sum();

                if (isKiai) totalSum *= 1.5f;

                if (triangles != null)
                    triangles.Velocity = 1 + totalSum;

                if (trianglesV2 != null)
                    trianglesV2.Velocity = 1 + totalSum;
            }

            private partial class TrianglesV1Wrapper(int? seed = null) : Triangles(seed)
            {
                protected override float SpawnRatio { get; } = 0.55f;
            }
        }

        protected override void PopIn()
        {
            backgroundTriangle.FadeIn(250);
        }

        protected override void PopOut()
        {
            backgroundTriangle.FadeOut(250);
        }
    }
}
