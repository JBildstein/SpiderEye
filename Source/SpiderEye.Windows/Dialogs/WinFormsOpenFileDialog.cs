using WFOpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace SpiderEye.Windows
{
    internal class WinFormsOpenFileDialog : WinFormsFileDialog<WFOpenFileDialog>, IOpenFileDialog
    {
        public bool Multiselect { get; set; }

        public string[] SelectedFiles
        {
            get;
            private set;
        }

        protected override WFOpenFileDialog GetDialog()
        {
            return new WFOpenFileDialog
            {
                Multiselect = Multiselect,
            };
        }

        protected override void BeforeReturn(WFOpenFileDialog dialog)
        {
            base.BeforeReturn(dialog);
            SelectedFiles = dialog.FileNames;
        }
    }
}
