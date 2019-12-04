using SpiderEye.Tools;
using SpiderEye.Windows.Interop;

namespace SpiderEye.Windows
{
    internal abstract class WinFormsDialog<T> : IDialog
        where T : System.Windows.Forms.CommonDialog
    {
        public string Title { get; set; }

        public DialogResult Show()
        {
            return Show(null);
        }

        public DialogResult Show(IWindow parent)
        {
            var dialog = GetDialog();
            BeforeShow(dialog);

            var window = NativeCast.To<WinFormsWindow>(parent);
            var result = dialog.ShowDialog(window);

            BeforeReturn(dialog);

            return WinFormsMapper.MapResult(result);
        }

        protected abstract T GetDialog();

        protected virtual void BeforeShow(T dialog)
        {
        }

        protected virtual void BeforeReturn(T dialog)
        {
        }
    }
}
