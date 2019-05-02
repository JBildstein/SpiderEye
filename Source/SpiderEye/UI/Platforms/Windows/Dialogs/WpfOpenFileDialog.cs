using Microsoft.Win32;

namespace SpiderEye.UI.Windows.Dialogs
{
    internal class WpfOpenFileDialog : WpfFileDialog, IOpenFileDialog
    {
        public bool Multiselect { get; set; }

        public string[] SelectedFiles
        {
            get;
            private set;
        }

        protected override FileDialog GetDialog()
        {
            return new OpenFileDialog
            {
                Multiselect = Multiselect,
            };
        }

        protected override void BeforeReturn(FileDialog dialog)
        {
            SelectedFiles = dialog.FileNames;
        }
    }
}
