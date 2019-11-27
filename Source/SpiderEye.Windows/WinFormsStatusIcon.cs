using System.Windows.Forms;
using SpiderEye.Tools;

namespace SpiderEye.Windows
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

        public Menu Menu
        {
            get { return menu; }
            set
            {
                menu = value;
                UpdateMenu(value);
            }
        }

        private readonly NotifyIcon notifyIcon;
        private AppIcon icon;
        private Menu menu;

        public WinFormsStatusIcon(string title)
        {
            notifyIcon = new NotifyIcon();
            Title = title;
            notifyIcon.Visible = true;
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

        private void UpdateMenu(Menu menu)
        {
            var nativeMenu = NativeCast.To<WinFormsMenu>(menu?.NativeMenu);
            notifyIcon.ContextMenu = nativeMenu?.Menu;
        }
    }
}
