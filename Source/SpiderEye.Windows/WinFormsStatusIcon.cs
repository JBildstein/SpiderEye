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

        public AppIcon Icon
        {
            get { return icon; }
            set
            {
                icon = value;
                UpdateIcon(value);
            }
        }

        private readonly NotifyIcon notifyIcon;
        private AppIcon icon;

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

        private void UpdateIcon(AppIcon icon)
        {
            if (icon == null || icon.Icons.Length == 0)
            {
                notifyIcon.Icon = null;
            }
            else
            {
                using (var stream = icon.GetIconDataStream(icon.DefaultIcon))
                {
                    notifyIcon.Icon = new System.Drawing.Icon(stream);
                }
            }
        }
    }
}
