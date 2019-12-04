using SpiderEye.Bridge;

namespace SpiderEye.Mac
{
    internal class CocoaUiFactory : IUiFactory
    {
        public IWindow CreateWindow(WindowConfiguration config, WebviewBridge bridge)
        {
            return new CocoaWindow(config, bridge);
        }

        public IStatusIcon CreateStatusIcon(string title)
        {
            return new CocoaStatusIcon(title);
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

        public IFolderSelectDialog CreateFolderSelectDialog()
        {
            return new CocoaFolderSelectDialog();
        }

        public IMenu CreateMenu()
        {
            return new CocoaMenu();
        }

        public ILabelMenuItem CreateLabelMenu(string label)
        {
            return new CocoaLabelMenuItem(label);
        }

        public IMenuItem CreateMenuSeparator()
        {
            return new CocoaSeparatorMenuItem();
        }
    }
}
