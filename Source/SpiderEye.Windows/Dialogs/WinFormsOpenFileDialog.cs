using WFFileDialog = System.Windows.Forms.FileDialog;
using WFOpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace SpiderEye.Windows
{
    internal class WinFormsOpenFileDialog : WinFormsFileDialog, IOpenFileDialog
    {
        public bool Multiselect { get; set; }

        public string[] SelectedFiles
        {
            get;
            private set;
        }

        protected override WFFileDialog GetDialog()
        {
            return new WFOpenFileDialog
            {
                Multiselect = Multiselect,
            };
        }

        protected override void BeforeReturn(WFFileDialog dialog)
        {
            SelectedFiles = dialog.FileNames;
        }
    }
}
