using System;
using SpiderEye.UI.Linux.Interop;
using SpiderEye.UI.Linux.Native;

namespace SpiderEye.UI.Linux.Menu
{
    internal class GtkLabelMenuItem : GtkMenuItem, ILabelMenuItem
    {
        public event EventHandler Click;

        public string Label
        {
            get { return GLibString.FromPointer(Gtk.Menu.GetMenuItemLabel(Handle)); }
            set
            {
                using (GLibString label = value)
                {
                    Gtk.Menu.SetMenuItemLabel(Handle, label);
                }
            }
        }

        public bool Enabled
        {
            get { return Gtk.Widget.GetEnabled(Handle); }
            set { Gtk.Widget.SetEnabled(Handle, value); }
        }

        public GtkLabelMenuItem(string label)
            : base(CreateHandle(label))
        {
            GLib.ConnectSignal(Handle, "activate", (MenuActivateDelegate)MenuActivatedCallback, IntPtr.Zero);
        }

        protected override void AddItem(IntPtr item)
        {
            Gtk.Menu.AddSubmenu(Handle, item);
        }

        private static IntPtr CreateHandle(string label)
        {
            using (GLibString glabel = label)
            {
                return Gtk.Menu.CreateLabelItem(glabel);
            }
        }

        private void MenuActivatedCallback(IntPtr menu, IntPtr userdata)
        {
            Click?.Invoke(this, EventArgs.Empty);
        }
    }
}
