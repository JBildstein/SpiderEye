using System;
using SpiderEye.Configuration;
using SpiderEye.UI.Mac.Dialogs;

namespace SpiderEye.UI.Mac
{
    internal class CocoaWindowFactory : IWindowFactory
    {
        private readonly AppConfiguration appConfig;

        public CocoaWindowFactory(AppConfiguration appConfig)
        {
            this.appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
        }

        public IWindow CreateWindow(WindowConfiguration config)
        {
            return new CocoaWindow(appConfig.CopyWithWindow(config));
        }

        public IMessageBox CreateMessageBox()
        {
            return new CocoaMessageBox();
        }

        public ISaveFileDialog CreateSaveFileDialog()
        {
            return new CocoaSaveFileDialog();
        }

        public IOpenFileDialog CreateOpenFileDialog()
        {
            return new CocoaOpenFileDialog();
        }
    }
}
