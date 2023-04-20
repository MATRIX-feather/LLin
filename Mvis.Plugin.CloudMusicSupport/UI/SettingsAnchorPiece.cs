using System.Linq;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Game.Rulesets.IGPlayer.Player.SideBar.Settings.Items;

namespace Mvis.Plugin.CloudMusicSupport.UI
{
    public partial class SettingsAnchorPiece : SettingsListPiece<Anchor>
    {
        public SettingsAnchorPiece()
        {
            var anchorArray = new[]
            {
                Anchor.TopLeft,
                Anchor.TopCentre,
                Anchor.TopRight,
                Anchor.CentreLeft,
                Anchor.Centre,
                Anchor.CentreRight,
                Anchor.BottomLeft,
                Anchor.BottomCentre,
                Anchor.BottomRight,
            };

            Values = anchorArray.ToList();
        }

        protected override string GetValueText(Anchor newValue) => newValue.GetDescription();
    }
}
