using System;
using SpiderEye.Configuration;
using SpiderEye.UI.Linux.Dialogs;

namespace SpiderEye.UI.Linux
{
    internal class GtkWindowFactory : IWindowFactory
    {
        private readonly AppConfiguration appConfig;

        public GtkWindowFactory(AppConfiguration appConfig)
        {
            this.appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
        }

        public IWindow CreateWindow(WindowConfiguration config)
        {
            return new GtkWindow(appConfig.CopyWithWindow(config), this);
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
    }
}
