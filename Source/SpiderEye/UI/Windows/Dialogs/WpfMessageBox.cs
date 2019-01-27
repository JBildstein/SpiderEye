using System;
using System.Windows;

namespace SpiderEye.UI.Windows.Dialogs
{
    internal class WpfMessageBox : IMessageBox
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public MessageBoxButtons Buttons { get; set; }

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

        private MessageBoxButton MapButtons(MessageBoxButtons buttons)
        {
            switch (buttons)
            {
                case MessageBoxButtons.Ok:
                    return MessageBoxButton.OK;
                case MessageBoxButtons.OkCancel:
                    return MessageBoxButton.OKCancel;
                case MessageBoxButtons.YesNo:
                    return MessageBoxButton.YesNo;
                case MessageBoxButtons.YesNoCancel:
                    return MessageBoxButton.YesNoCancel;

                default:
                    return MessageBoxButton.OK;
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
