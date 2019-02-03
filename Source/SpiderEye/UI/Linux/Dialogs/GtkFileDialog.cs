using System;
using System.Collections.Generic;
using System.Linq;
using SpiderEye.UI.Linux.Interop;
using SpiderEye.UI.Linux.Native;

namespace SpiderEye.UI.Linux.Dialogs
{
    internal abstract class GtkFileDialog : IFileDialog
    {
        public string Title { get; set; }
        public string InitialDirectory { get; set; }
        public string FileName { get; set; }
        public ICollection<FileFilter> FileFilters { get; }

        protected abstract GtkFileChooserAction Type { get; }

        public GtkFileDialog()
        {
            FileFilters = new List<FileFilter>();
        }

        public DialogResult Show()
        {
            return Show(null);
        }

        public DialogResult Show(IWindow parent)
        {
            var window = parent as GtkWindow;
            if (parent != null && window == null)
            {
                throw new ArgumentException("Invalid window type.", nameof(parent));
            }

            bool useNative = false && Gtk.Version.IsAtLeast(3, 2, 0);
            IntPtr dialog = IntPtr.Zero;
            try
            {
                using (GLibString gtitle = Title)
                {
                    if (useNative)
                    {
                        dialog = Gtk.Dialog.CreateNativeFileDialog(
                            gtitle.Pointer,
                            window?.Handle ?? IntPtr.Zero,
                            Type,
                            IntPtr.Zero,
                            IntPtr.Zero);
                    }
                    else
                    {
                        string acceptString = GetAcceptString(Type);
                        using (GLibString acceptButton = acceptString)
                        using (GLibString cancelButton = "_Cancel")
                        {
                            dialog = Gtk.Dialog.CreateFileDialog(
                               gtitle.Pointer,
                               window?.Handle ?? IntPtr.Zero,
                               Type,
                               cancelButton,
                               GtkResponseType.Cancel,
                               acceptButton,
                               GtkResponseType.Accept,
                               IntPtr.Zero);
                        }
                    }
                }

                Gtk.Dialog.SetCanCreateFolder(dialog, true);
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

                BeforeShow(dialog);

                GtkResponseType result;
                if (useNative) { result = Gtk.Dialog.RunNative(dialog); }
                else { result = Gtk.Dialog.Run(dialog); }

                using (var fileName = new GLibString(Gtk.Dialog.GetFileName(dialog)))
                {
                    FileName = fileName.ToString();
                }

                BeforeReturn(dialog);

                return MapResult(result);
            }
            finally
            {
                if (dialog != IntPtr.Zero)
                {
                    if (useNative) { GLib.UnrefObject(dialog); }
                    else { Gtk.Widget.Destroy(dialog); }
                }
            }
        }

        protected virtual void BeforeShow(IntPtr dialog)
        {
        }

        protected virtual void BeforeReturn(IntPtr dialog)
        {
        }

        private string GetAcceptString(GtkFileChooserAction type)
        {
            switch (type)
            {
                case GtkFileChooserAction.Open:
                    return "_Open";

                case GtkFileChooserAction.Save:
                    return "_Save";

                case GtkFileChooserAction.SelectFolder:
                default:
                    return "_Select";
            }
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

        private DialogResult MapResult(GtkResponseType result)
        {
            switch (result)
            {
                case GtkResponseType.Accept:
                case GtkResponseType.Ok:
                case GtkResponseType.Yes:
                case GtkResponseType.Apply:
                    return DialogResult.Ok;

                case GtkResponseType.Reject:
                case GtkResponseType.Cancel:
                case GtkResponseType.Close:
                case GtkResponseType.No:
                    return DialogResult.Cancel;

                default:
                    return DialogResult.None;
            }
        }
    }
}
