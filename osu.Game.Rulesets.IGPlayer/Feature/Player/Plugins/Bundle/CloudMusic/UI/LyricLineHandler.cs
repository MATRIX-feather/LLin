using System.Linq;
using System.Threading;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.Config;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.UI
{
    public partial class LyricLineHandler : CompositeDrawable
    {
        private OsuSpriteText? currentLine;
        private OsuSpriteText? currentLineTranslated;
        private string currentRawText = string.Empty;
        private string currentRawTranslateText = string.Empty;
        private readonly BufferedContainer outlineEffectContainer;

        private readonly Container lyricContainer = new Container
        {
            //Shear = new Vector2(0.25f, 0) * 0.7f,

            AutoSizeAxes = Axes.Both,
            Anchor = Anchor.BottomCentre,
            Origin = Anchor.BottomCentre,
            Margin = new MarginPadding { Horizontal = 15, Top = 15 }
        };

        private Easing fadeOutEasing => Easing.OutQuint;
        private Easing fadeInEasing => Easing.OutQuint;

        private CancellationTokenSource? textCancellationTokenSource;

        private CancellationTokenSource? translateTextCancellationTokenSource;

        public string Text
        {
            get => currentRawText;
            set
            {
                currentLine?.MoveToY(5, fadeOutDuration.Value, fadeOutEasing)
                           .FadeOut(fadeOutDuration.Value, fadeOutEasing).Then().Expire();

                currentLine = null;

                textCancellationTokenSource?.Cancel();

                var tokenSource = new CancellationTokenSource();
                textCancellationTokenSource = tokenSource;

                Schedule(() =>
                {
                    LoadComponentAsync(new OsuSpriteText
                    {
                        Text = value,
                        Alpha = 0,
                        Y = -5,
                        Anchor = configDirection.Value,
                        Origin = configDirection.Value,
                        Font = OsuFont.GetFont(size: 30, weight: FontWeight.Black),
                        Margin = getMargin(false)
                    }, text =>
                    {
                        if (tokenSource.IsCancellationRequested)
                            return;

                        lyricContainer.Add(text);
                        text.MoveToY(0, fadeInDuration.Value, fadeInEasing)
                            .FadeIn(fadeInDuration.Value, fadeInEasing);

                        currentLine = text;
                    }, tokenSource.Token);
                });

                currentRawText = value;
                checkIfEmpty();
            }
        }

        public string TranslatedText
        {
            get => currentRawTranslateText;
            set
            {
                currentLineTranslated?.MoveToY(5, fadeOutDuration.Value, fadeOutEasing)
                                     .FadeOut(fadeOutDuration.Value, fadeOutEasing)
                                     .Then()
                                     .Expire();

                currentLineTranslated = null;

                this.translateTextCancellationTokenSource?.Cancel();

                var tokenSource = new CancellationTokenSource();
                this.translateTextCancellationTokenSource = tokenSource;

                Schedule(() =>
                {
                    LoadComponentAsync(new OsuSpriteText
                    {
                        Text = value,
                        Alpha = 0,
                        Y = -5,
                        Font = OsuFont.GetFont(size: 30, weight: FontWeight.Black),
                        Anchor = configDirection.Value,
                        Origin = configDirection.Value,
                        Margin = getMargin(true)
                    }, text =>
                    {
                        if (tokenSource.IsCancellationRequested)
                            return;

                        text.MoveToY(0, fadeInDuration.Value, fadeInEasing)
                            .FadeIn(fadeInDuration.Value, fadeInEasing);

                        lyricContainer.Add(text);
                        currentLineTranslated = text;
                    }, tokenSource.Token);
                });

                currentRawTranslateText = value;
                checkIfEmpty();
            }
        }

        private void checkIfEmpty()
        {
            if (LoadState < LoadState.Ready)
            {
                Schedule(checkIfEmpty);
                return;
            }

            if (string.IsNullOrEmpty(Text) && string.IsNullOrEmpty(TranslatedText))
                this.FadeOut(200, Easing.OutQuint);
            else
                this.FadeIn(200, Easing.OutQuint);
        }

        public LyricLineHandler()
        {
            RelativeSizeAxes = Axes.Both;
            RelativePositionAxes = Axes.Both;
            Alpha = 0;

            //Padding = new MarginPadding { Right = 50 - 15, Bottom = 50 - 20 - 5 };

            InternalChild = outlineEffectContainer = new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
            }.WithEffect(new OutlineEffect
            {
                Colour = Color4.Black.Opacity(0.3f),
                BlurSigma = new Vector2(3f),
                Strength = 3f
            });

            outlineEffectContainer.FrameBufferScale = new Vector2(1.1f);
        }

        private readonly Bindable<float> fadeInDuration = new Bindable<float>();
        private readonly Bindable<float> fadeOutDuration = new Bindable<float>();
        private readonly Bindable<bool> disableOutline = new Bindable<bool>();
        private readonly Bindable<float> positionX = new Bindable<float>();
        private readonly Bindable<float> positionY = new Bindable<float>();

        private readonly Bindable<Anchor> configDirection = new Bindable<Anchor>
        {
            Default = Anchor.BottomCentre,
            Value = Anchor.BottomCentre,
        };

        [BackgroundDependencyLoader]
        private void load(LyricConfigManager config)
        {
            //var config = Dependencies.Get<LyricConfigManager>();

            config.BindWith(LyricSettings.LyricFadeInDuration, fadeInDuration);
            config.BindWith(LyricSettings.LyricFadeOutDuration, fadeOutDuration);
            config.BindWith(LyricSettings.NoExtraShadow, disableOutline);

            config.BindWith(LyricSettings.LyricDirection, configDirection);
            config.BindWith(LyricSettings.LyricPositionX, positionX);
            config.BindWith(LyricSettings.LyricPositionY, positionY);
        }

        protected override void LoadComplete()
        {
            disableOutline.BindValueChanged(v =>
            {
                if (v.NewValue)
                {
                    if (outlineEffectContainer.Contains(lyricContainer))
                        outlineEffectContainer.Remove(lyricContainer, false);

                    AddInternal(lyricContainer);
                    outlineEffectContainer.Hide();
                }
                else
                {
                    if (InternalChildren.Contains(lyricContainer))
                        RemoveInternal(lyricContainer, false);

                    outlineEffectContainer.Add(lyricContainer);
                    outlineEffectContainer.Show();
                }
            }, true);

            configDirection.BindValueChanged(v =>
            {
                if (currentLineTranslated != null)
                {
                    currentLineTranslated.Anchor = v.NewValue;
                    currentLineTranslated.Origin = v.NewValue;
                    currentLineTranslated.Margin = getMargin(true);
                }

                if (currentLine != null)
                {
                    currentLine.Anchor = v.NewValue;
                    currentLine.Origin = v.NewValue;
                    currentLine.Margin = getMargin(false);
                }

                lyricContainer.Anchor = lyricContainer.Origin = v.NewValue;
                outlineEffectContainer.Anchor = outlineEffectContainer.Origin = v.NewValue;
            }, true);

            positionX.BindValueChanged(v => this.MoveToX(v.NewValue, 300, Easing.OutQuint));
            positionY.BindValueChanged(v => this.MoveToY(v.NewValue, 300, Easing.OutQuint));

            base.LoadComplete();
        }

        private MarginPadding getMargin(bool isTranslateText)
        {
            bool revert = configDirection.Value == Anchor.TopCentre
                          || configDirection.Value == Anchor.TopLeft
                          || configDirection.Value == Anchor.TopRight;

            const int amount = 40;

            //在顶上：原始歌词Margin.Top=0，翻译Margin.Top=40
            //在底下：原始歌词Margin.Top=40，翻译Margin.Top=0

            if (isTranslateText)
                return new MarginPadding { Bottom = revert ? 0 : amount, Top = revert ? amount : 0 };

            return new MarginPadding { Bottom = revert ? amount : 0, Top = revert ? 0 : amount };
        }
    }
}
