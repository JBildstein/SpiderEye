using SpiderEye.UI.Mac.Dialogs;

namespace SpiderEye.UI.Mac
{
    internal class CocoaUiFactory : IUiFactory
    {
        public IWindow CreateWindow(WindowConfiguration config)
        {
            return new CocoaWindow(config, this);
        }

        public IStatusIcon CreateStatusIcon()
        {
            return new CocoaStatusIcon();
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
