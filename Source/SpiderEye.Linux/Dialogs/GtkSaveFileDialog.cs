using System;
using SpiderEye.Linux.Interop;
using SpiderEye.Linux.Native;

namespace SpiderEye.Linux
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
