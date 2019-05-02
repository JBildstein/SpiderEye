using SpiderEye.UI.Windows.Dialogs;

namespace SpiderEye.UI.Windows
{
    internal class WpfUiFactory : IUiFactory
    {
        public IWindow CreateWindow(WindowConfiguration config)
        {
            return new WpfWindow(config, this);
        }

        public IStatusIcon CreateStatusIcon()
        {
            return new WinFormsStatusIcon();
        }

        public IMessageBox CreateMessageBox()
        {
            return new WpfMessageBox();
        }

        public ISaveFileDialog CreateSaveFileDialog()
        {
            return new WpfSaveFileDialog();
        }

        public IOpenFileDialog CreateOpenFileDialog()
        {
            return new WpfOpenFileDialog();
        }
    }
}
