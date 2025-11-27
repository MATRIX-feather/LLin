using System;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Overlays.Dialog;
using osu.Game.Resources.Localisation.Web;
using osu.Game.Rulesets.Hikariii.Localisation.LLin;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Graphics
{
    public partial class AudioControlRequestDialog : PopupDialog
    {
        public AudioControlRequestDialog(LocalisableString source, LocalisableString reason, Action? onConfirm, Action? onDeny)
        {
            HeaderText = LLinBaseStrings.AudioControlRequestedMain(source);
            BodyText = LLinBaseStrings.AudioControlRequestedSub(reason);

            Icon = FontAwesome.Solid.ExclamationTriangle;

            Buttons = new PopupDialogButton[]
            {
                new PopupDialogOkButton
                {
                    Text = LLinBaseStrings.Okay,
                    Action = onConfirm,
                },
                new PopupDialogCancelButton
                {
                    Text = CommonStrings.ButtonsCancel,
                    Action = onDeny,
                },
            };
        }
    }
}
