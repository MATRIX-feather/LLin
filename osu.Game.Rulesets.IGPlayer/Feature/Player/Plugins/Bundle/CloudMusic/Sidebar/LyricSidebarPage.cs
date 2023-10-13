using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Screens;
using osu.Game.Beatmaps;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Graphics;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.Sidebar.Graphic;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.Sidebar.Screens;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Types;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Screens.LLin;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.Sidebar
{
    public partial class LyricSidebarSectionContainer : PluginSidebarPage
    {
        private LoadingSpinner loading = null!;

        public LyricSidebarSectionContainer(LLinPlugin plugin)
            : base(plugin)
        {
            Icon = FontAwesome.Solid.Music;
        }

        public override IPluginFunctionProvider GetFunctionEntry()
            => new LyricFunctionProvider(this);

        [Resolved]
        private IImplementLLin mvisScreen { get; set; } = null!;

        private LyricPlugin plugin => (LyricPlugin)Plugin;

        public int BeatmapSetId;

        private ScreenStack screenStack = null!;

        private Toolbox toolbox = null!;

        [BackgroundDependencyLoader]
        private void load(CustomColourProvider provider)
        {
            Children = new Drawable[]
            {
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Direction = FillDirection.Horizontal,
                    Children = new Drawable[]
                    {
                        screenStack = new SidebarScreenStack
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.TopRight,
                            Origin = Anchor.TopRight,
                            Width = 0.4f
                        },
                        toolbox = new Toolbox
                        {
                            OnBackAction = screenStack.Exit
                        }
                    }
                },
                loading = new LoadingLayer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                }
            };

            plugin.CurrentStatus.BindValueChanged(v =>
            {
                switch (v.NewValue)
                {
                    case LyricPlugin.Status.Finish:
                    case LyricPlugin.Status.Failed:
                        loading.Hide();
                        break;

                    default:
                        loading.Show();
                        break;
                }
            }, true);

            screenStack.ScreenPushed += onScreenChanged;
            screenStack.ScreenExited += onScreenChanged;
        }

        private void onScreenChanged(IScreen lastscreen, IScreen newscreen)
        {
            if (newscreen is SidebarScreen screen)
                toolbox.AddButtonRange(screen.Entries, (screen is LyricViewScreen));
        }

        protected override void LoadComplete()
        {
            mvisScreen.OnBeatmapChanged(refreshBeatmap, this);
            refreshBeatmap(mvisScreen.Beatmap.Value);

            screenStack.Push(new LyricViewScreen());
            base.LoadComplete();
        }

        private void refreshBeatmap(WorkingBeatmap working)
        {
            BeatmapSetId = working.BeatmapSetInfo.OnlineID;
            toolbox.IdText = $"ID: {BeatmapSetId}";
        }
    }
}
