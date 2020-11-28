using FormsDialogButtons = System.Windows.Forms.MessageBoxButtons;
using FormsDialogResult = System.Windows.Forms.DialogResult;

namespace SpiderEye.Windows.Interop
{
    internal static class WinFormsMapper
    {
        public static DialogResult MapResult(FormsDialogResult result)
        {
            return result switch
            {
                FormsDialogResult.None => DialogResult.None,
                FormsDialogResult.OK => DialogResult.Ok,
                FormsDialogResult.Cancel => DialogResult.Cancel,
                FormsDialogResult.Yes => DialogResult.Yes,
                FormsDialogResult.No => DialogResult.No,
                _ => DialogResult.None,
            };
        }

        public static FormsDialogButtons MapButtons(MessageBoxButtons buttons)
        {
            return buttons switch
            {
                MessageBoxButtons.Ok => FormsDialogButtons.OK,
                MessageBoxButtons.OkCancel => FormsDialogButtons.OKCancel,
                MessageBoxButtons.YesNo => FormsDialogButtons.YesNo,
                _ => FormsDialogButtons.OK,
            };
        }
    }
}
