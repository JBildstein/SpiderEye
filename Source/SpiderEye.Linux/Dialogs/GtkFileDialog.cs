using System;
using System.Collections.Generic;
using System.Linq;
using SpiderEye.Linux.Interop;
using SpiderEye.Linux.Native;

namespace SpiderEye.Linux
{
    internal abstract class GtkFileDialog : GtkDialog, IFileDialog
    {
        public string InitialDirectory { get; set; }
        public string FileName { get; set; }
        public ICollection<FileFilter> FileFilters { get; }

        protected GtkFileDialog()
        {
            FileFilters = new List<FileFilter>();
        }

        protected override void BeforeShow(IntPtr dialog)
        {
            if (!string.IsNullOrWhiteSpace(InitialDirectory))
            {
                using (GLibString dir = InitialDirectory)
                {
                    Gtk.Dialog.SetCurrentFolder(dialog, dir);
                }
            }

            if (!string.IsNullOrWhiteSpace(FileName))
            {
                using (GLibString name = FileName)
                {
                    Gtk.Dialog.SetFileName(dialog, name);
                }
            }

            SetFileFilters(dialog, FileFilters);
        }

        protected override void BeforeReturn(IntPtr dialog, DialogResult result)
        {
            if (result == DialogResult.Ok)
            {
                using (var fileName = new GLibString(Gtk.Dialog.GetFileName(dialog)))
                {
                    FileName = fileName.ToString();
                }
            }
            else { FileName = null; }
        }

        private void SetFileFilters(IntPtr dialog, IEnumerable<FileFilter> filters)
        {
            if (!filters.Any()) { return; }

            foreach (var filter in filters)
            {
                var gfilter = Gtk.Dialog.FileFilter.Create();
                using (GLibString name = filter.Name)
                {
                    Gtk.Dialog.FileFilter.SetName(gfilter, name);
                }

                foreach (string filterValue in filter.Filters)
                {
                    using (GLibString value = filterValue)
                    {
                        Gtk.Dialog.FileFilter.AddPattern(gfilter, value);
                    }
                }

                Gtk.Dialog.AddFileFilter(dialog, gfilter);
            }
        }
    }
}
