using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SideBar.Settings.Items;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.BottomBar.Buttons;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Bundle.BottomBar
{
    internal partial class LegacyBottomBar : LLinPlugin, IFunctionBarProvider
    {
        protected override Drawable CreateContent() => new PlaceHolder();

        protected override bool OnContentLoaded(Drawable content) => true;

        protected override bool PostInit() => true;

        private readonly FillFlowContainer<BottomBarButton> leftContent;
        private readonly FillFlowContainer<BottomBarButton> centreContent;
        private readonly FillFlowContainer<BottomBarButton> rightContent;
        private readonly FillFlowContainer<BottomBarButton> floatingContent;

        private readonly SongProgressBar progressBar;
        private readonly Container contentContainer;

        public override int Version => 10;

        public override TargetLayer Target => TargetLayer.FunctionBar;

        public LegacyBottomBar()
        {
            Name = "底栏";
            Description = "mf-osu默认功能条";
            Author = "MATRIX-夜翎";
            Depth = -1;

            Flags.AddRange(new[]
            {
                PluginFlags.CanUnload
            });

            Anchor = Anchor.BottomCentre;
            Origin = Anchor.BottomCentre;
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;

            InternalChildren = new Drawable[]
            {
                contentContainer = new Container
                {
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    AutoSizeDuration = 300,
                    AutoSizeEasing = Easing.OutQuint,
                    Children = new Drawable[]
                    {
                        leftContent = new FillFlowContainer<BottomBarButton>
                        {
                            Name = "Left Container",
                            Anchor = Anchor.BottomLeft,
                            Origin = Anchor.BottomLeft,
                            AutoSizeAxes = Axes.Both,
                            Spacing = new Vector2(5),
                            Margin = new MarginPadding { Left = 5, Bottom = 10 }
                        },
                        centreContent = new FillFlowContainer<BottomBarButton>
                        {
                            Name = "Centre Container",
                            Anchor = Anchor.BottomCentre,
                            Origin = Anchor.BottomCentre,
                            AutoSizeAxes = Axes.Both,
                            Spacing = new Vector2(2.5f),
                            Scale = new Vector2(1.25f),
                            Margin = new MarginPadding { Bottom = 10 }
                        },
                        rightContent = new FillFlowContainer<BottomBarButton>
                        {
                            Name = "Right Container",
                            Anchor = Anchor.BottomRight,
                            Origin = Anchor.BottomRight,
                            AutoSizeAxes = Axes.Both,
                            Spacing = new Vector2(5),
                            Margin = new MarginPadding { Right = 5, Bottom = 10 }
                        },
                        floatingContent = new FillFlowContainer<BottomBarButton>
                        {
                            Name = "Plugin Entries Container",
                            Anchor = Anchor.BottomCentre,
                            Origin = Anchor.BottomCentre,
                            AutoSizeAxes = Axes.Both,
                            AutoSizeDuration = 300,
                            AutoSizeEasing = Easing.OutQuint,
                            Spacing = new Vector2(5)
                        },
                    }
                },
                progressBar = new SongProgressBar()
            };
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            LLin!.OnIdle += Hide;
            LLin.OnActive += Show;

            progressBar.OnSeek = target =>
            {
                if (!LLin.SeekTo(target)) progressBar.FlashColour(Color4.Red, 1000, Easing.OutQuint);
            };
        }

        protected override void Update()
        {
            progressBar.CurrentTime = LLin!.CurrentTrack.CurrentTime;
            progressBar.EndTime = LLin.CurrentTrack.Length;

            float margin = 0f;

            foreach (var drawable in contentContainer.Children)
            {
                if (drawable.Equals(floatingContent)) continue;

                margin = Math.Max(margin, drawable.Margin.Bottom + drawable.Height);
            }

            floatingContent.Margin = new MarginPadding { Bottom = margin + 15 };

            base.Update();
        }

        public float GetSafeAreaPadding() => contentContainer.Height - contentContainer.Y + 10;

        public override void Show()
        {
            contentContainer.MoveToY(0, 300, Easing.OutQuint);
            progressBar.MoveToY(0, 300, Easing.OutQuint);
        }

        public override void Hide()
        {
            contentContainer.MoveToY(floatingContent.Margin.Bottom - 5, 300, Easing.OutQuint);
            progressBar.MoveToY(4f, 300, Easing.OutQuint);
        }

        public bool OkForHide() => !IsHovered;

        private void checkForPluginControls(IFunctionProvider provider)
        {
            if (provider is IPluginFunctionProvider pluginFunctionProvider)
                pluginButtons.Add(pluginFunctionProvider);
        }

        public bool AddFunctionControl(IFunctionProvider provider)
        {
            checkForPluginControls(provider);

            var button = provider is IToggleableFunctionProvider functionProvider
                ? new BottomBarSwitchButton(functionProvider)
                : new BottomBarButton(provider);

            switch (provider.Type)
            {
                case FunctionType.Audio:
                    button.ShearStrength = 0.1f;
                    centreContent.Add(button);
                    break;

                case FunctionType.Base:
                    leftContent.Add(button);
                    break;

                case FunctionType.Misc:
                    rightContent.Add(button);
                    break;

                case FunctionType.Plugin:
                    floatingContent.Add(button);
                    break;

                case FunctionType.ProgressDisplay:
                    button.Dispose();

                    var newButton = new SongProgressButton((IToggleableFunctionProvider)provider)
                    {
                        ShearStrength = 0.1f
                    };

                    centreContent.Add(newButton);
                    break;

                default:
                    throw new InvalidOperationException("???");
            }

            provider.OnActive = success =>
            {
                if (!success) button.FlashColour(Color4.Red, 1000, Easing.OutQuint);
                else button.DoFlash();
            };

            return true;
        }

        public bool AddFunctionControls(List<IFunctionProvider> providers)
        {
            foreach (var provider in providers)
            {
                AddFunctionControl(provider);
            }

            return true;
        }

        public bool SetFunctionControls(List<IFunctionProvider> providers)
        {
            leftContent.Clear();
            centreContent.Clear();
            rightContent.Clear();
            floatingContent.Clear();

            return AddFunctionControls(providers);
        }

        private readonly List<IPluginFunctionProvider> pluginButtons = new List<IPluginFunctionProvider>();

        public void Remove(IFunctionProvider provider)
        {
            BottomBarButton? target;

            switch (provider.Type)
            {
                case FunctionType.ProgressDisplay:
                case FunctionType.Audio:
                    target = centreContent.FirstOrDefault(b => b.Provider == provider);
                    break;

                case FunctionType.Base:
                    target = leftContent.FirstOrDefault(b => b.Provider == provider);
                    break;

                case FunctionType.Misc:
                    target = rightContent.FirstOrDefault(b => b.Provider == provider);
                    break;

                case FunctionType.Plugin:
                    target = floatingContent.FirstOrDefault(b => b.Provider == provider);
                    break;

                default:
                    throw new InvalidOperationException("???");
            }

            if (target != null)
            {
                target.Expire();

                if (provider is IPluginFunctionProvider pluginFunctionProvider)
                    pluginButtons.Remove(pluginFunctionProvider);
            }
            else
                throw new ButtonNotFoundException(provider);
        }

        public void ShowFunctionControlTemporary() => floatingContent.FadeIn(500, Easing.OutQuint).Then().Delay(2000).FadeOut(500, Easing.OutQuint);

        public List<IPluginFunctionProvider> GetAllPluginFunctionButton() => pluginButtons;
        public Action? OnDisable { get; set; }

        public override bool Disable()
        {
            OnDisable?.Invoke();
            return base.Disable();
        }

        public class ButtonNotFoundException : Exception
        {
            public ButtonNotFoundException(IFunctionProvider provider)
                : base($"无法找到与{provider.ToString()}对应的按钮")
            {
            }
        }
    }
}
