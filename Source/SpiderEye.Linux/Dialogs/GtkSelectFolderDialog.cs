using System;
using SpiderEye.Linux.Interop;
using SpiderEye.Linux.Native;

namespace SpiderEye.Linux
{
    internal class GtkSelectFolderDialog : GtkDialog, IFolderSelectDialog
    {
        public string? SelectedPath { get; set; }

        protected override GtkFileChooserAction Type
        {
            // SelectFolder doesn't allow creating a new folder, CreateFolder does
            get { return GtkFileChooserAction.CreateFolder; }
        }

        protected override void BeforeShow(IntPtr dialog)
        {
            if (!string.IsNullOrWhiteSpace(SelectedPath))
            {
                using GLibString dir = SelectedPath;
                Gtk.Dialog.SetCurrentFolder(dialog, dir);
            }
        }

        protected override unsafe void BeforeReturn(IntPtr dialog, DialogResult result)
        {
            if (result == DialogResult.Ok)
            {
                using var folderPath = new GLibString(Gtk.Dialog.GetFileName(dialog));
                SelectedPath = folderPath.ToString();
            }
            else { SelectedPath = null; }
        }
    }
}
