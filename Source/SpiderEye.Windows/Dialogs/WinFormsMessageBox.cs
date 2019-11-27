using SpiderEye.Tools;
using SpiderEye.Windows.Interop;
using WFMessageBox = System.Windows.Forms.MessageBox;

namespace SpiderEye.Windows
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
            var window = NativeCast.To<WinFormsWindow>(parent);
            System.Windows.Forms.DialogResult result;
            if (window == null)
            {
                result = WFMessageBox.Show(
                    Message,
                    Title,
                    WinFormsMapper.MapButtons(Buttons));
            }
            else
            {
                result = WFMessageBox.Show(
                    window,
                    Message,
                    Title,
                    WinFormsMapper.MapButtons(Buttons));
            }

            return WinFormsMapper.MapResult(result);
        }
    }
}
