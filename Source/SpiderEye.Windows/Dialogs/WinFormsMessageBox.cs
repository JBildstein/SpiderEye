using System;
using System.Windows.Forms;
using SpiderEye.UI.Windows.Interop;

namespace SpiderEye.UI.Windows.Dialogs
{
    internal class WinFormsMessageBox : IMessageBox
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
            var window = parent as WinFormsWindow;
            if (parent != null && window == null)
            {
                throw new ArgumentException("Invalid window type.", nameof(parent));
            }

            System.Windows.Forms.DialogResult result;
            if (window == null)
            {
                result = MessageBox.Show(
                    Message,
                    Title,
                    WinFormsMapper.MapButtons(Buttons));
            }
            else
            {
                result = MessageBox.Show(
                    window,
                    Message,
                    Title,
                    WinFormsMapper.MapButtons(Buttons));
            }

            return WinFormsMapper.MapResult(result);
        }
    }
}
