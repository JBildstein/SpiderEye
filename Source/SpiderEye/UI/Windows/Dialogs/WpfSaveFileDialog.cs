using Microsoft.Win32;

namespace SpiderEye.UI.Windows.Dialogs
{
    internal class WpfSaveFileDialog : WpfFileDialog, ISaveFileDialog
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
