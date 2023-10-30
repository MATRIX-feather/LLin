using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Logging;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Online.API;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Misc;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.Yasp.Config;
using osu.Game.Users.Drawables;
using osuTK;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.Yasp.Panels;

public partial class CoverIIPanel : CompositeDrawable, IPanel
{
    private WorkingBeatmap currentBeatmap;

    public void Refresh(WorkingBeatmap beatmap)
    {
        this.currentBeatmap = beatmap;

        var meta = beatmap?.Metadata ?? new BeatmapMetadata();
        titleText.Text = displayUnicode.Value ? meta.GetTitle() : meta.Title;
        artistText.Text = displayUnicode.Value ? meta.GetArtist() : meta.Artist;

        sourceText.Text = string.IsNullOrEmpty(meta.Source)
            ? displayUnicode.Value
                ? meta.Title
                : meta.GetTitle()
            : meta.Source;

        cover?.Refresh(useUserAvatar.Value, beatmap);
    }

    private readonly BindableBool useUserAvatar = new BindableBool();
    private readonly BindableBool displayUnicode = new BindableBool();

    private readonly TruncatingSpriteText titleText = new TruncatingSpriteText
    {
        Padding = new MarginPadding { Bottom = -5 },
        MaxWidth = 650,
        AllowMultiline = false,
        Font = OsuFont.GetFont(size: 60, weight: FontWeight.Bold, typeface: Typeface.TorusAlternate)
    };

    private readonly TruncatingSpriteText artistText = new TruncatingSpriteText
    {
        MaxWidth = 600,
        AllowMultiline = false,
        Font = OsuFont.GetFont(size: 36, weight: FontWeight.Medium),
        Padding = new MarginPadding { Bottom = 0 },
    };

    private readonly TruncatingSpriteText sourceText = new TruncatingSpriteText
    {
        MaxWidth = 600,
        AllowMultiline = false,
        Font = OsuFont.GetFont(size: 24, weight: FontWeight.Medium)
    };

    private AvatarOrBeatmapCover? cover;

    [BackgroundDependencyLoader]
    private void load(YaspPlugin plugin, FrameworkConfigManager frameworkConfig)
    {
        var config = (YaspConfigManager)Dependencies.Get<LLinPluginManager>().GetConfigManager(plugin);
        Logger.Log($"{config}");
        config.BindWith(YaspSettings.CoverIIUseUserAvatar, useUserAvatar);
        frameworkConfig.BindWith(FrameworkSetting.ShowUnicode, displayUnicode);

        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        AutoSizeAxes = Axes.Both;

        int squareLength = 168;
        var container = new FillFlowContainer
        {
            AutoSizeAxes = Axes.X,
            AutoSizeDuration = 300,
            AutoSizeEasing = Easing.OutQuint,
            Direction = FillDirection.Horizontal,

            X = 2,
            Height = squareLength,

            Spacing = new Vector2(25),

            Children = new Drawable[]
            {
                new FillFlowContainer
                {
                    Name = "Cover flow",
                    Direction = FillDirection.Horizontal,
                    RelativeSizeAxes = Axes.Y,
                    AutoSizeAxes = Axes.X,

                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Y,
                            Width = 7.5f
                        },
                        cover = new AvatarOrBeatmapCover
                        {
                            RelativeSizeAxes = Axes.Y,
                            Width = squareLength
                        },
                    }
                },
                new Container
                {
                    Name = "Line",
                    RelativeSizeAxes = Axes.Y,
                    Width = 2,

                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Height = 0.8f,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    }
                },
                new FillFlowContainer
                {
                    Name = "Detail Flow",
                    Direction = FillDirection.Vertical,
                    RelativeSizeAxes = Axes.Y,
                    AutoSizeAxes = Axes.X,

                    Margin = new MarginPadding { Top = 15 },

                    Children = new Drawable[]
                    {
                        titleText,
                        artistText,
                        sourceText
                    }
                }
            }
        };

        this.AddInternal(container);
        useUserAvatar.BindValueChanged(v =>
        {
            Refresh(this.currentBeatmap);
        });

        displayUnicode.BindValueChanged(v =>
        {
            Refresh(this.currentBeatmap);
        }, true);
    }

    private partial class AvatarOrBeatmapCover : CompositeDrawable
    {
        private WorkingBeatmap? workingBeatmap;
        private bool useUserAvatar;

        [Resolved(canBeNull: true)]
        private IAPIProvider? api { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            this.Masking = true;
        }

        public void Refresh(bool useUserAvatar, WorkingBeatmap? workingBeatmap)
        {
            Drawable child;

            if (useUserAvatar && this.useUserAvatar)
                return;

            bool useAvatarChanged = this.useUserAvatar != useUserAvatar;

            if (this.workingBeatmap == workingBeatmap && !useAvatarChanged)
                return;

            this.useUserAvatar = useUserAvatar;
            this.workingBeatmap = workingBeatmap;

            if (useUserAvatar)
            {
                child = new UpdateableAvatar(api?.LocalUser.Value ?? null);
            }
            else
            {
                child = new BeatmapCover(workingBeatmap)
                {
                    TimeBeforeWrapperLoad = 0,
                    UseBufferedBackground = false,
                    BackgroundBox = false
                };
            }

            child.RelativeSizeAxes = Axes.Both;
            InternalChildren = new Drawable[]
            {
                new Box
                {
                    Depth = float.MaxValue,
                    RelativeSizeAxes = Axes.Both,
                    Colour = ColourInfo.GradientVertical(Color4Extensions.FromHex("#555"), Color4Extensions.FromHex("#444")),
                },
                child
            };
        }
    }
}
