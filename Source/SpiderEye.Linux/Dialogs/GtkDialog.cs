using System;
using SpiderEye.Linux.Interop;
using SpiderEye.Linux.Native;
using SpiderEye.Tools;

namespace SpiderEye.Linux
{
    internal abstract class GtkDialog : IDialog
    {
        public string Title { get; set; }

        protected abstract GtkFileChooserAction Type { get; }

        public DialogResult Show()
        {
            return Show(null);
        }

        public DialogResult Show(IWindow parent)
        {
            var window = NativeCast.To<GtkWindow>(parent);
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

                BeforeShow(dialog);

                GtkResponseType result;
                if (useNative) { result = Gtk.Dialog.RunNative(dialog); }
                else { result = Gtk.Dialog.Run(dialog); }

                var mappedResult = MapResult(result);
                BeforeReturn(dialog, mappedResult);

                return mappedResult;
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

        protected virtual void BeforeReturn(IntPtr dialog, DialogResult result)
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
