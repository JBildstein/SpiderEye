using System;
using SpiderEye.UI.Linux.Native;
using SpiderEye.UI.Platforms.Linux.Interop;

namespace SpiderEye.UI.Linux.Menu
{
    internal abstract class GtkMenuItem : IMenuItem
    {
        public readonly IntPtr Handle;

        protected GtkMenuItem(IntPtr handle)
        {
            Handle = handle;
        }

        public ILabelMenuItem AddLabelMenuItem(string label)
        {
            var item = new GtkLabelMenuItem(label);
            AddItem(item.Handle);
            Gtk.Widget.Show(item.Handle);

            return item;
        }

        public void AddSeparatorMenuItem()
        {
            var item = Gtk.Menu.CreateSeparatorItem();
            AddItem(item);
            Gtk.Widget.Show(item);
        }

        public void Dispose()
        {
            Gtk.Widget.Destroy(Handle);
        }

        public void SetShortcut(ModifierKey modifier, Key key)
        {
            SetShortcut(KeyMapper.GetShortcut(modifier, key));
        }

        protected abstract void AddItem(IntPtr item); // TODO: keep references to the subitems somewhere (to avoid garbage collection issues)

        protected abstract void SetShortcut(string shortcut);
    }
}
