using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SpiderEye.Linux.Interop;
using SpiderEye.Linux.Native;

namespace SpiderEye.Linux
{
    internal class GtkOpenFileDialog : GtkFileDialog, IOpenFileDialog
    {
        public bool Multiselect { get; set; }

        public string[]? SelectedFiles
        {
            get;
            private set;
        }

        protected override GtkFileChooserAction Type
        {
            get { return GtkFileChooserAction.Open; }
        }

        protected override void BeforeShow(IntPtr dialog)
        {
            base.BeforeShow(dialog);
            Gtk.Dialog.SetAllowMultiple(dialog, Multiselect);
        }

        protected override unsafe void BeforeReturn(IntPtr dialog, DialogResult result)
        {
            base.BeforeReturn(dialog, result);

            var filesPtr = Gtk.Dialog.GetSelectedFiles(dialog);
            var files = new List<string>();
            if (filesPtr != IntPtr.Zero)
            {
                try
                {
                    var list = Marshal.PtrToStructure<GSList>(filesPtr);
                    while (true)
                    {
                        using (var value = new GLibString(list.Data))
                        {
                            files.Add(value.ToString()!);
                        }

                        if (list.Next == null) { break; }
                        list = *list.Next;
                    }
                }
                finally { GLib.FreeSList(filesPtr); }
            }

            SelectedFiles = files.ToArray();
        }
    }
}
