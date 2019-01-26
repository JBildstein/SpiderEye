using System;

namespace SpiderEye.UI.Linux.Dialogs
{
    internal class GtkMessageBox : IMessageBox
    {
        public string Title { get; set; }
        public string Message { get; set; }

        public MessageBoxButton Buttons { get; set; }

        public DialogResult Show()
        {
            throw new NotImplementedException();
        }

        public DialogResult Show(IWindow parent)
        {
            throw new NotImplementedException();
        }
    }
}
