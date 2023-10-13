using System;
using osu.Framework.Graphics.Pooling;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.Misc;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.Sidebar.Graphic
{
    public abstract partial class DrawableLyric : PoolableDrawable, IComparable<DrawableLyric>
    {
        public Lyric Value
        {
            get => value;
            set
            {
                if (IsLoaded)
                    UpdateValue(value);

                this.value = value;
            }
        }

        private Lyric value = null!;

        public float CurrentY;
        public abstract int FinalHeight();

        protected override void LoadComplete()
        {
            UpdateValue(value);
            base.LoadComplete();
        }

        protected abstract void UpdateValue(Lyric lyric);

        public int CompareTo(DrawableLyric? other) => CurrentY.CompareTo(other?.CurrentY ?? 0);
    }
}
