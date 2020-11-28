using System;
using SpiderEye.Linux.Interop;
using SpiderEye.Linux.Native;
using SpiderEye.UI.Platforms.Linux.Interop;

namespace SpiderEye.Linux
{
    internal class GtkLabelMenuItem : GtkMenuItem, ILabelMenuItem
    {
        public event EventHandler? Click;

        public string? Label
        {
            get { return GLibString.FromPointer(Gtk.Menu.GetMenuItemLabel(Handle)); }
            set
            {
                using GLibString label = value;
                Gtk.Menu.SetMenuItemLabel(Handle, label);
            }
        }

        public bool Enabled
        {
            get { return Gtk.Widget.GetEnabled(Handle); }
            set { Gtk.Widget.SetEnabled(Handle, value); }
        }

        private readonly MenuActivateDelegate menuActivateDelegate;

        public GtkLabelMenuItem(string label)
            : base(CreateHandle(label))
        {
            // need to keep the delegate around or it will get garbage collected
            menuActivateDelegate = MenuActivatedCallback;
            GLib.ConnectSignal(Handle, "activate", menuActivateDelegate, IntPtr.Zero);
        }

        public void SetShortcut(ModifierKey modifier, Key key)
        {
            string shortcut = KeyMapper.GetShortcut(modifier, key);
            using GLibString gshortcut = shortcut;
            Gtk.Menu.SetAccelerator(Handle, gshortcut);
        }

        private static IntPtr CreateHandle(string label)
        {
            using GLibString glabel = label;
            return Gtk.Menu.CreateLabelItem(glabel);
        }

        private void MenuActivatedCallback(IntPtr menu, IntPtr userdata)
        {
            Click?.Invoke(this, EventArgs.Empty);
        }
    }
}
