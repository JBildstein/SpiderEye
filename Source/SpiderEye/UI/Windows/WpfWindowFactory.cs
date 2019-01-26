using System;
using SpiderEye.Configuration;
using SpiderEye.UI.Windows.Dialogs;

namespace SpiderEye.UI.Windows
{
    internal class WpfWindowFactory : IWindowFactory
    {
        private readonly AppConfiguration appConfig;

        public WpfWindowFactory(AppConfiguration appConfig)
        {
            this.appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
        }

        public IWindow CreateWindow(WindowConfiguration config)
        {
            return new WpfWindow(appConfig.CopyWithWindow(config), this);
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
