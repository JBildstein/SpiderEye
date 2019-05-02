using SpiderEye.UI.Mac.Interop;

namespace SpiderEye.UI.Mac.Dialogs
{
    internal class CocoaSaveFileDialog : CocoaFileDialog, ISaveFileDialog
    {
        public bool OverwritePrompt { get; set; }

        protected override NSDialog CreateDialog()
        {
            // TODO: can't disable overwrite prompt on macOS
            return NSDialog.CreateSavePanel();
        }
    }
}
