using System.IO;
using System.Windows.Forms;
using SpiderEye.UI.Windows.Menu;

namespace SpiderEye.UI.Windows
{
    internal class WinFormsStatusIcon : IStatusIcon
    {
        public string Title
        {
            get { return notifyIcon.Text; }
            set { notifyIcon.Text = value; }
        }

        public Icon Icon
        {
            get { return icon; }
            set
            {
                icon = value;
                UpdateIcon(value);
            }
        }

        private readonly NotifyIcon notifyIcon;
        private Icon icon;

        public WinFormsStatusIcon()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.Visible = true;
        }

        public IMenu AddMenu()
        {
            var menu = new WinFormsMenu();
            notifyIcon.ContextMenu = menu.Menu;

            return menu;
        }

        public void Dispose()
        {
            notifyIcon.Dispose();
        }

        private void UpdateIcon(Icon icon)
        {
            if (icon == null || icon.Icons.Count == 0)
            {
                notifyIcon.Icon = null;
            }
            else
            {
                using (var stream = new MemoryStream(icon.Icons[0]))
                {
                    notifyIcon.Icon = new System.Drawing.Icon(stream);
                }
            }
        }
    }
}
