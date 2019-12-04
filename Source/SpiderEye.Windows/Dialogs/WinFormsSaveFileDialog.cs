using WFSaveFileDialog = System.Windows.Forms.SaveFileDialog;

namespace SpiderEye.Windows
{
    internal class WinFormsSaveFileDialog : WinFormsFileDialog<WFSaveFileDialog>, ISaveFileDialog
    {
        public bool OverwritePrompt { get; set; }

        protected override WFSaveFileDialog GetDialog()
        {
            return new WFSaveFileDialog
            {
                OverwritePrompt = OverwritePrompt,
            };
        }
    }
}
