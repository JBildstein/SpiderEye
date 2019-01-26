using Microsoft.Win32;

namespace SpiderEye.UI.Windows.Dialogs
{
    internal class WpfOpenFileDialog : WpfFileDialog, IOpenFileDialog
    {
        public bool Multiselect { get; set; }

        protected override FileDialog GetDialog()
        {
            return new OpenFileDialog
            {
                Multiselect = Multiselect,
            };
        }
    }
}
