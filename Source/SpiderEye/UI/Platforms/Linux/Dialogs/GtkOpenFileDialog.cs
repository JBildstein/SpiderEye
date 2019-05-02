using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SpiderEye.UI.Linux.Interop;
using SpiderEye.UI.Linux.Native;

namespace SpiderEye.UI.Linux.Dialogs
{
    internal class GtkOpenFileDialog : GtkFileDialog, IOpenFileDialog
    {
        public bool Multiselect { get; set; }

        public string[] SelectedFiles
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
            Gtk.Dialog.SetAllowMultiple(dialog, Multiselect);
        }

        protected override unsafe void BeforeReturn(IntPtr dialog)
        {
            var ptr = Gtk.Dialog.GetSelectedFiles(dialog);
            var result = new List<string>();
            if (ptr != IntPtr.Zero)
            {
                try
                {
                    var list = Marshal.PtrToStructure<GSList>(ptr);
                    while (true)
                    {
                        using (var value = new GLibString(list.Data))
                        {
                            result.Add(value.ToString());
                        }

                        if (list.Next == null) { break; }
                        list = *list.Next;
                    }
                }
                finally { GLib.FreeSList(ptr); }
            }

            SelectedFiles = result.ToArray();
        }
    }
}
