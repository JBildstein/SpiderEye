using System;
using SpiderEye.UI.Linux.Dialogs;

namespace SpiderEye.UI.Linux
{
    internal class GtkUiFactory : IUiFactory
    {
        public IWindow CreateWindow(WindowConfiguration config)
        {
            return new GtkWindow(config, this);
        }

        public IStatusIcon CreateStatusIcon()
        {
            return new GtkStatusIcon();
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
