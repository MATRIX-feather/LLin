using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.Backgrounds;
using osu.Game.Rulesets.Hikariii.Features.Configuration;
using osu.Game.Rulesets.Hikariii.ppyStuffs.osu;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Screens.LLin
{
    public partial class MBgTriangles : VisibilityContainer
    {
        private readonly Bindable<bool> optui = new Bindable<bool>();
        private readonly BackgroundTriangles backgroundTriangle;

        [BackgroundDependencyLoader]
        private void load(MConfigManager config)
        {
            config.BindWith(MSetting.OptUI, optui);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            optui.BindValueChanged(updateIcons, true);
        }

        public MBgTriangles(float alpha = 0.65f, bool highLight = false, float triangleScale = 2f, bool sync = false)
        {
            Alpha = alpha;
            Masking = true;
            RelativeSizeAxes = Axes.Both;
            Anchor = Anchor.BottomCentre;
            Origin = Anchor.BottomCentre;
            Children = new Drawable[]
            {
                backgroundTriangle = new BackgroundTriangles(highLight, triangleScale)
                {
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    AllowBeatSync = sync
                },
            };
        }

        private partial class BackgroundTriangles : Container
        {
            private Triangles? triangles;
            private TrianglesV2Copy? trianglesV2;
            public readonly float TriangleScale;

            public bool AllowBeatSync;

            private readonly bool highLight;

            public BackgroundTriangles(bool highLight = false, float triangleScaleValue = 2f)
            {
                RelativeSizeAxes = Axes.Both;
                TriangleScale = triangleScaleValue;

                this.highLight = highLight;
            }

            private readonly Bindable<bool> useV2 = new Bindable<bool>();

            [BackgroundDependencyLoader]
            private void load(MConfigManager configManager)
            {
                configManager.BindWith(MSetting.MvisUseTriangleV2, useV2);

                InternalChildren =
                [
                    trianglesV2 ??= new TrianglesV2Copy
                    {
                        Anchor = Anchor.BottomCentre,
                        Origin = Anchor.BottomCentre,
                        RelativeSizeAxes = Axes.Both,
                        Colour = highLight ? Color4Extensions.FromHex(@"88b300") : OsuColour.Gray(0.2f),
                        Alpha = 0,
                        SpawnRatio = 1,
                        ExtraScaleRange = (0.2f, 2f)
                    },
                    triangles ??= new TrianglesV1Wrapper
                    {
                        Anchor = Anchor.BottomCentre,
                        Origin = Anchor.BottomCentre,
                        RelativeSizeAxes = Axes.Both,
                        TriangleScale = TriangleScale,
                        Colour = highLight ? Color4Extensions.FromHex(@"88b300") : OsuColour.Gray(0.2f),
                        Alpha = 0
                    }
                ];

                useV2.BindValueChanged(v => updateTriangle(v.NewValue), true);
            }

            private void updateTriangle(bool useV2)
            {
                if (useV2)
                {
                    this.trianglesV2.FadeIn(300, Easing.OutQuint);
                    this.triangles.FadeOut(300, Easing.OutQuint);
                }
                else
                {
                    this.trianglesV2.FadeOut(300, Easing.OutQuint);
                    this.triangles.FadeIn(300, Easing.OutQuint);
                }
            }

            [Resolved]
            private IBindable<WorkingBeatmap> b { get; set; } = null!;

            protected override void Update()
            {
                if (!AllowBeatSync) return;

                float[] sum = b.Value?.Track.CurrentAmplitudes.FrequencyAmplitudes.ToArray() ?? new float[256];
                float totalSum = 0.1f;
                bool isKiai = b.Value?.Beatmap.ControlPointInfo.EffectPointAt(b.Value?.Track?.CurrentTime ?? 0).KiaiMode ?? false;

                sum.ForEach(a => totalSum += a);

                if (isKiai) totalSum *= 1.5f;

                if (triangles != null)
                    triangles.Velocity = 1 + totalSum;

                if (trianglesV2 != null)
                    trianglesV2.Velocity = 1 + totalSum;
            }

            private partial class TrianglesV1Wrapper : Triangles
            {
                protected override float SpawnRatio { get; } = 0.55f;
            }
        }

        private void updateIcons(ValueChangedEvent<bool> value)
        {
            switch (value.NewValue)
            {
                case true:
                    backgroundTriangle.FadeIn(250);
                    break;

                case false:
                    backgroundTriangle.FadeOut(250);
                    break;
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
