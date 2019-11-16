using SpiderEye.UI.Windows.Dialogs;

namespace SpiderEye.UI.Windows
{
    internal class WinFormsUiFactory : IUiFactory
    {
        public IWindow CreateWindow(WindowConfiguration config)
        {
            return new WinFormsWindow(config, this);
        }

        public IStatusIcon CreateStatusIcon()
        {
            return new WinFormsStatusIcon();
        }

        public IMessageBox CreateMessageBox()
        {
            return new WinFormsMessageBox();
        }

        public ISaveFileDialog CreateSaveFileDialog()
        {
            return new WinFormsSaveFileDialog();
        }

        public IOpenFileDialog CreateOpenFileDialog()
        {
            return new WinFormsOpenFileDialog();
        }
    }
}
