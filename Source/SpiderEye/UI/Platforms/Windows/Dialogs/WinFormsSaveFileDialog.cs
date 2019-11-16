using System.Windows.Forms;

namespace SpiderEye.UI.Windows.Dialogs
{
    internal class WinFormsSaveFileDialog : WinFormsFileDialog, ISaveFileDialog
    {
        public bool OverwritePrompt { get; set; }

        protected override FileDialog GetDialog()
        {
            return new SaveFileDialog
            {
                OverwritePrompt = OverwritePrompt,
            };
        }
    }
}
