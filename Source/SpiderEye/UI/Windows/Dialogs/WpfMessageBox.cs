using System;
using System.Windows;

namespace SpiderEye.UI.Windows.Dialogs
{
    internal class WpfMessageBox : IMessageBox
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public MessageBoxButton Buttons { get; set; }

        public DialogResult Show()
        {
            return Show(null);
        }

        public DialogResult Show(IWindow parent)
        {
            var window = parent as WpfWindow;
            if (parent != null && window == null)
            {
                throw new ArgumentException("Invalid window type.", nameof(parent));
            }

            MessageBoxResult result;
            if (window == null)
            {
                result = MessageBox.Show(
                    Message,
                    Title,
                    MapButtons(Buttons));
            }
            else
            {
                result = MessageBox.Show(
                    window,
                    Message,
                    Title,
                    MapButtons(Buttons));
            }

            return MapResult(result);
        }

        private System.Windows.MessageBoxButton MapButtons(MessageBoxButton buttons)
        {
            switch (buttons)
            {
                case MessageBoxButton.Ok:
                    return System.Windows.MessageBoxButton.OK;
                case MessageBoxButton.OkCancel:
                    return System.Windows.MessageBoxButton.OKCancel;
                case MessageBoxButton.YesNo:
                    return System.Windows.MessageBoxButton.YesNo;
                case MessageBoxButton.YesNoCancel:
                    return System.Windows.MessageBoxButton.YesNoCancel;

                default:
                    return System.Windows.MessageBoxButton.OK;
            }
        }

        private DialogResult MapResult(MessageBoxResult result)
        {
            switch (result)
            {
                case MessageBoxResult.None:
                    return DialogResult.None;

                case MessageBoxResult.OK:
                    return DialogResult.Ok;

                case MessageBoxResult.Cancel:
                    return DialogResult.Cancel;

                case MessageBoxResult.Yes:
                    return DialogResult.Yes;

                case MessageBoxResult.No:
                    return DialogResult.No;

                default:
                    return DialogResult.None;
            }
        }
    }
}
