using System;
using SpiderEye.UI.Linux.Interop;
using SpiderEye.UI.Linux.Native;

namespace SpiderEye.UI.Linux.Dialogs
{
    internal class GtkSaveFileDialog : GtkFileDialog, ISaveFileDialog
    {
        public bool OverwritePrompt { get; set; }

        protected override GtkFileChooserAction Type
        {
            get { return GtkFileChooserAction.Save; }
        }

        protected override void BeforeShow(IntPtr dialog)
        {
            Gtk.Dialog.SetOverwriteConfirmation(dialog, OverwritePrompt);
        }
    }
}
