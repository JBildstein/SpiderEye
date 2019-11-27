using WFFileDialog = System.Windows.Forms.FileDialog;
using WFSaveFileDialog = System.Windows.Forms.SaveFileDialog;

namespace SpiderEye.Windows
{
    internal class WinFormsSaveFileDialog : WinFormsFileDialog, ISaveFileDialog
    {
        public bool OverwritePrompt { get; set; }

        protected override WFFileDialog GetDialog()
        {
            return new WFSaveFileDialog
            {
                OverwritePrompt = OverwritePrompt,
            };
        }
    }
}
