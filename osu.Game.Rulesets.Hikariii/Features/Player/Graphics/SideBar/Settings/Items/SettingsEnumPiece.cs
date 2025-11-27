using System;
using System.Linq;
using osu.Framework.Extensions;
using osu.Framework.Localisation;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SideBar.Settings.Items
{
    public partial class SettingsEnumPiece<T> : SettingsListPiece<T>
        where T : struct, Enum
    {
        public SettingsEnumPiece()
        {
            var array = (T[])Enum.GetValues(typeof(T));
            Values = array.ToList();
        }

        protected override LocalisableString GetValueText(T newValue) => newValue.GetLocalisableDescription();
    }
}
