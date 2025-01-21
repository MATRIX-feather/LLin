#nullable disable

using System;
using System.Numerics;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SideBar.Settings.Items
{
    public partial class SettingsSliderPiece<T> : SettingsPieceBasePanel, ISettingsItem<T>
        where T : struct, INumber<T>, IMinMaxValue<T>, IConvertible
    {
        public Bindable<T> Bindable { get; set; }

        public LocalisableString TooltipText
        {
            get => tooltip;
            set => tooltip = value + " (点按中键重置)";
        }

        private string tooltip = "点按中键重置";

        public bool DisplayAsPercentage;
        public bool TransferValueOnCommit;

        protected override IconUsage DefaultIcon => FontAwesome.Solid.SlidersH;

        protected override Drawable CreateSideDrawable() => new SettingsSlider<T>
        {
            RelativeSizeAxes = Axes.Both,
            Current = Bindable,
            DisplayAsPercentage = DisplayAsPercentage,
            TransferValueOnCommit = TransferValueOnCommit,
        };

        protected override void OnMiddleClick()
        {
            Bindable.Value = Bindable.Default;
        }
    }
}
