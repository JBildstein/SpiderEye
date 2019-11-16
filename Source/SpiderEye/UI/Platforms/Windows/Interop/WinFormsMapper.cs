using FormsDialogButtons = System.Windows.Forms.MessageBoxButtons;
using FormsDialogResult = System.Windows.Forms.DialogResult;

namespace SpiderEye.UI.Windows.Interop
{
    internal static class WinFormsMapper
    {
        public static DialogResult MapResult(FormsDialogResult result)
        {
            switch (result)
            {
                case FormsDialogResult.None:
                    return DialogResult.None;

                case FormsDialogResult.OK:
                    return DialogResult.Ok;

                case FormsDialogResult.Cancel:
                    return DialogResult.Cancel;

                case FormsDialogResult.Yes:
                    return DialogResult.Yes;

                case FormsDialogResult.No:
                    return DialogResult.No;

                default:
                    return DialogResult.None;
            }
        }

        public static FormsDialogButtons MapButtons(MessageBoxButtons buttons)
        {
            switch (buttons)
            {
                case MessageBoxButtons.Ok:
                    return FormsDialogButtons.OK;
                case MessageBoxButtons.OkCancel:
                    return FormsDialogButtons.OKCancel;
                case MessageBoxButtons.YesNo:
                    return FormsDialogButtons.YesNo;

                default:
                    return FormsDialogButtons.OK;
            }
        }
    }
}
