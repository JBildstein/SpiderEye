using System;
using SpiderEye.UI.Linux.Interop;
using SpiderEye.UI.Linux.Native;

namespace SpiderEye.UI.Linux.Dialogs
{
    internal class GtkMessageBox : IMessageBox
    {
        public string Title { get; set; }
        public string Message { get; set; }

        public MessageBoxButtons Buttons { get; set; }

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

            IntPtr dialog = IntPtr.Zero;
            try
            {
                using (GLibString title = Title)
                using (GLibString message = Message)
                {
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
            }
            finally { if (dialog != IntPtr.Zero) { Gtk.Widget.Destroy(dialog); } }
        }

        private GtkButtonsType MapButtons(MessageBoxButtons buttons)
        {
            switch (buttons)
            {
                case MessageBoxButtons.Ok:
                    return GtkButtonsType.Ok;
                case MessageBoxButtons.OkCancel:
                    return GtkButtonsType.OkCancel;
                case MessageBoxButtons.YesNo:
                    return GtkButtonsType.YesNo;

                default:
                    return GtkButtonsType.Ok;
            }
        }

        private DialogResult MapResult(GtkResponseType result)
        {
            switch (result)
            {
                case GtkResponseType.Ok:
                    return DialogResult.Ok;

                case GtkResponseType.Cancel:
                    return DialogResult.Cancel;

                case GtkResponseType.Yes:
                    return DialogResult.Yes;

                case GtkResponseType.No:
                    return DialogResult.No;

                default:
                    return DialogResult.None;
            }
        }
    }
}
