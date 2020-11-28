using System;
using SpiderEye.Linux.Interop;
using SpiderEye.Linux.Native;
using SpiderEye.Tools;

namespace SpiderEye.Linux
{
    internal class GtkMessageBox : IMessageBox
    {
        public string? Title { get; set; }
        public string? Message { get; set; }
        public MessageBoxButtons Buttons { get; set; }

        public DialogResult Show()
        {
            return Show(null);
        }

        public DialogResult Show(IWindow? parent)
        {
            var window = NativeCast.To<GtkWindow>(parent);
            IntPtr dialog = IntPtr.Zero;
            try
            {
                using GLibString title = Title;
                using GLibString message = Message;
                dialog = Gtk.Dialog.CreateMessageDialog(
                   window?.Handle ?? IntPtr.Zero,
                   GtkDialogFlags.Modal | GtkDialogFlags.DestroyWithParent,
                   GtkMessageType.Other,
                   MapButtons(Buttons),
                   IntPtr.Zero);

                GLib.SetProperty(dialog, "title", title);
                GLib.SetProperty(dialog, "text", message);

                var result = Gtk.Dialog.Run(dialog);
                return MapResult(result);
            }
            finally { if (dialog != IntPtr.Zero) { Gtk.Widget.Destroy(dialog); } }
        }

        private static GtkButtonsType MapButtons(MessageBoxButtons buttons)
        {
            return buttons switch
            {
                MessageBoxButtons.Ok => GtkButtonsType.Ok,
                MessageBoxButtons.OkCancel => GtkButtonsType.OkCancel,
                MessageBoxButtons.YesNo => GtkButtonsType.YesNo,
                _ => GtkButtonsType.Ok,
            };
        }

        private static DialogResult MapResult(GtkResponseType result)
        {
            return result switch
            {
                GtkResponseType.Ok => DialogResult.Ok,
                GtkResponseType.Cancel => DialogResult.Cancel,
                GtkResponseType.Yes => DialogResult.Yes,
                GtkResponseType.No => DialogResult.No,
                _ => DialogResult.None,
            };
        }
    }
}
