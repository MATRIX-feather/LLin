using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LLin.Game.Graphics.Cursor;
using LLin.Game.KeyBind;
using LLin.Game.Online;
using LLin.Game.Screens;
using LLin.Game.Screens.Mvis;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.IO.Stores;
using osuTK;
using LLin.Resources;
using M.Resources;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Textures;
using osu.Framework.Platform;
using osu.Game;
using osu.Game.Audio;
using osu.Game.Beatmaps;
using osu.Game.Collections;
using osu.Game.Configuration;
using osu.Game.Database;
using osu.Game.Graphics;
using osu.Game.Input;
using osu.Game.IO;
using osu.Game.Online.API;
using osu.Game.Overlays;
using osu.Game.Resources;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Mods;
using osu.Game.Scoring;
using osu.Game.Utils;
using MConfigManager = LLin.Game.Configuration.MConfigManager;

namespace LLin.Game
{
    public class LLinGameBase : osu.Framework.Game
    {
        // Anything in this class is shared between the test browser and the game implementation.
        // It allows for caching global dependencies that should be accessible to tests, or changing
        // the screen scaling for all components including the test browser and framework overlays.

        protected override Container<Drawable> Content => content;

        private Container content;

        protected LLinGameBase()
        {
        }

        private DependencyContainer dependencies;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        private WorkingBeatmap defaultBeatmap;
        private DatabaseContextFactory contextFactory;

        protected IAPIProvider APIAccess { get; set; }
        protected OsuConfigManager OsuConfig { get; set; }
        protected RulesetStore OsuRulesetStore { get; set; }
        protected MusicController OsuMusicController { get; set; }
        protected Storage Storage { get; set; }
        protected BeatmapManager BeatmapManager { get; set; }

        [Cached]
        [Cached(typeof(IBindable<RulesetInfo>))]
        protected readonly Bindable<RulesetInfo> Ruleset = new Bindable<RulesetInfo>();

        [Cached]
        [Cached(typeof(IBindable<IReadOnlyList<Mod>>))]
        protected readonly Bindable<IReadOnlyList<Mod>> SelectedMods = new Bindable<IReadOnlyList<Mod>>(Array.Empty<Mod>());

        private RealmContextFactory realmFactory;

        [BackgroundDependencyLoader]
        private void load()
        {
            Resources.AddStore(new DllResourceStore(LLinResources.ResourceAssembly));
            Resources.AddStore(new DllResourceStore(MResources.ResourceAssembly));
            Resources.AddStore(new DllResourceStore(OsuResources.ResourceAssembly));

            dependencies.CacheAs(new MConfigManager(Storage));
            dependencies.CacheAs(Storage);

            LargeTextureStore largeTextureStore;
            dependencies.CacheAs(largeTextureStore = new LargeTextureStore());
            largeTextureStore.AddStore(Host.CreateTextureLoaderStore(new NamespacedResourceStore<byte[]>(Resources, @"Textures")));

            CustomColourProvider customColorProvider;
            dependencies.CacheAs(customColorProvider = new CustomColourProvider());
            AddInternal(customColorProvider);

            var cursorContainer = new LLinCursorContainer { RelativeSizeAxes = Axes.Both };
            cursorContainer.Add(content = new LLinTooltipContainer(cursorContainer.Cursor) { RelativeSizeAxes = Axes.Both });

            // Ensure game and tests scale with window size and screen DPI.
            base.Content.Add(new DrawSizePreservingFillContainer
            {
                // You may want to change TargetDrawSize to your "default" resolution, which will decide how things scale and position when using absolute coordinates.
                TargetDrawSize = new Vector2(1366, 768),
                Child = cursorContainer
            });

            //osu.Game兼容
            defaultBeatmap = new DummyWorkingBeatmap(Audio, null);

            dependencies.Cache(OsuConfig = new OsuConfigManager(Storage));

            dependencies.Cache(new SessionStatics());

            dependencies.Cache(new OsuColour());

            Resources.AddStore(new DllResourceStore(typeof(LLinResources).Assembly));

            dependencies.CacheAs(APIAccess ??= new APIAccess(OsuConfig, new VoidApiEndpointConfiguration(), string.Empty));

            dependencies.Cache(contextFactory = new DatabaseContextFactory(Storage));
            dependencies.Cache(realmFactory = new RealmContextFactory(Storage));

            dependencies.Cache(OsuRulesetStore = new RulesetStore(contextFactory, Storage)); //OsuScreen
            dependencies.Cache(new FileStore(contextFactory, Storage)); //由Storyboard使用

            dependencies.Cache(BeatmapManager = new BeatmapManager(Storage,
                contextFactory,
                OsuRulesetStore,
                APIAccess,
                Audio,
                Resources,
                Host,
                defaultBeatmap, true));

            dependencies.Cache(new CollectionManager(Storage));

            BeatmapDifficultyCache osuBeatmapDifficultyCache;
            dependencies.Cache(osuBeatmapDifficultyCache = new BeatmapDifficultyCache());
            Add(osuBeatmapDifficultyCache);

            dependencies.Cache(new ScoreManager(OsuRulesetStore,
                () => BeatmapManager,
                Storage,
                APIAccess,
                contextFactory,
                Scheduler,
                Host,
                () => osuBeatmapDifficultyCache,
                OsuConfig));

            dependencies.CacheAs(new OsuGameBase()); //SongSelect部分组件依赖

            var beatmap = new NonNullableBindable<WorkingBeatmap>(defaultBeatmap);

            dependencies.CacheAs<IBindable<WorkingBeatmap>>(beatmap);
            dependencies.CacheAs<Bindable<WorkingBeatmap>>(beatmap);

            //依赖BeatmapManager和IBindable<WorkingBeatmap>，放在最后
            AddInternal(OsuMusicController = new MusicController());
            dependencies.CacheAs(OsuMusicController);

            PreviewTrackManager osuPreviewTrackManager;
            dependencies.Cache(osuPreviewTrackManager = new PreviewTrackManager());
            Add(osuPreviewTrackManager);

            //global actions
            OsuGlobalActionContainer osuGlobalActionContainer;
            dependencies.Cache(osuGlobalActionContainer = new OsuGlobalActionContainer(this));

            LLinGlobalActionContainer llinGlobalActionContainer;
            dependencies.CacheAs(llinGlobalActionContainer = new LLinGlobalActionContainer());
            llinGlobalActionContainer.Child = osuGlobalActionContainer;

            var keyBindingStore = new RealmKeyBindingStore(realmFactory);
            keyBindingStore.Register(osuGlobalActionContainer, OsuRulesetStore.AvailableRulesets);

            base.Content.Add(llinGlobalActionContainer);

            //自定义字体
            dependencies.Cache(new CustomFontHelper());

            dependencies.Cache(new CustomStore(Storage, this));

            //osu字体
            AddFont(Resources, @"Fonts/osuFont");

            AddFont(Resources, @"Fonts/Torus/Torus-Regular");
            AddFont(Resources, @"Fonts/Torus/Torus-Light");
            AddFont(Resources, @"Fonts/Torus/Torus-SemiBold");
            AddFont(Resources, @"Fonts/Torus/Torus-Bold");

            AddFont(Resources, @"Fonts/Noto/Noto-Basic");
            AddFont(Resources, @"Fonts/Noto/Noto-Hangul");
            AddFont(Resources, @"Fonts/Noto/Noto-CJK-Basic");
            AddFont(Resources, @"Fonts/Noto/Noto-CJK-Compatibility");
            AddFont(Resources, @"Fonts/Noto/Noto-Thai");

            Ruleset.Value = OsuRulesetStore.AvailableRulesets.First();
        }

        public override void SetHost(GameHost host)
        {
            base.SetHost(host);

            Storage = host.Storage;

            if (Window != null)
            {
                Window.Title = "LLin";
            }

            if (Window is SDL2DesktopWindow sdl2DesktopWindow)
            {
                sdl2DesktopWindow.MinimumSize = new Size(1366, 768);
            }
        }
    }
}
