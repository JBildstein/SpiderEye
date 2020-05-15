using System;
using System.Windows.Forms;
using SpiderEye.Windows.Interop;

namespace SpiderEye.Windows
{
    internal class WinFormsLabelMenuItem : WinFormsMenuItem, ILabelMenuItem
    {
        public event EventHandler Click
        {
            add { menuItem.Click += value; }
            remove { menuItem.Click -= value; }
        }

        public bool Enabled
        {
            get { return menuItem.Enabled; }
            set { menuItem.Enabled = value; }
        }

        public string Label
        {
            get { return menuItem.Text; }
            set { menuItem.Text = value; }
        }

        public override ToolStripItem Item
        {
            get { return menuItem; }
        }

        private readonly ToolStripMenuItem menuItem;

        public WinFormsLabelMenuItem(string label)
        {
            menuItem = new ToolStripMenuItem(label);
        }

        public void SetShortcut(ModifierKey modifier, Key key)
        {
            var shortcut = KeyMapper.GetShortcut(modifier, key);
            menuItem.ShortcutKeys = shortcut;
        }

        public void SetSystemShorcut(SystemShortcut shortcut)
        {
            (var modifier, var key) = KeyMapper.ResolveSystemShortcut(shortcut);
            SetShortcut(modifier, key);
        }
    }
}
