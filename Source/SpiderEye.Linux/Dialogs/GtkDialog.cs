using System;
using SpiderEye.Linux.Interop;
using SpiderEye.Linux.Native;
using SpiderEye.Tools;

namespace SpiderEye.Linux
{
    internal abstract class GtkDialog : IDialog
    {
        public string? Title { get; set; }

        protected abstract GtkFileChooserAction Type { get; }

        public DialogResult Show()
        {
            return Show(null);
        }

        public DialogResult Show(IWindow? parent)
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
                        using GLibString acceptButton = acceptString;
                        using GLibString cancelButton = "_Cancel";
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

        private static string GetAcceptString(GtkFileChooserAction type)
        {
            return type switch
            {
                GtkFileChooserAction.Open => "_Open",
                GtkFileChooserAction.Save => "_Save",
                _ => "_Select",
            };
        }

        private static DialogResult MapResult(GtkResponseType result)
        {
            return result switch
            {
                GtkResponseType.Accept or GtkResponseType.Ok or GtkResponseType.Yes or GtkResponseType.Apply => DialogResult.Ok,
                GtkResponseType.Reject or GtkResponseType.Cancel or GtkResponseType.Close or GtkResponseType.No => DialogResult.Cancel,
                _ => DialogResult.None,
            };
        }
    }
}
