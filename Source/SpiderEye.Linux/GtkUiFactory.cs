using SpiderEye.Bridge;

namespace SpiderEye.Linux
{
    internal class GtkUiFactory : IUiFactory
    {
        public IWindow CreateWindow(WindowConfiguration config, WebviewBridge bridge)
        {
            return new GtkWindow(bridge);
        }

        public IStatusIcon CreateStatusIcon(string title)
        {
            return new GtkStatusIcon(title);
        }

        public IMessageBox CreateMessageBox()
        {
            return new GtkMessageBox();
        }

        public ISaveFileDialog CreateSaveFileDialog()
        {
            return new GtkSaveFileDialog();
        }

        public IOpenFileDialog CreateOpenFileDialog()
        {
            return new GtkOpenFileDialog();
        }

        public IMenu CreateMenu()
        {
            return new GtkMenu();
        }

        public ILabelMenuItem CreateLabelMenu(string label)
        {
            return new GtkLabelMenuItem(label);
        }

        public IMenuItem CreateMenuSeparator()
        {
            return new GtkSeparatorMenuItem();
        }
    }
}
