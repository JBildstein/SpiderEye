using SpiderEye.Mac.Interop;
using SpiderEye.Mac.Native;
using SpiderEye.Tools;

namespace SpiderEye.Mac
{
    internal abstract class CocoaDialog : IDialog
    {
        public string? Title { get; set; }

        public DialogResult Show()
        {
            return Show(null);
        }

        public DialogResult Show(IWindow? parent)
        {
            var window = NativeCast.To<CocoaWindow>(parent);
            var dialog = CreateDialog();

            ObjC.Call(dialog.Handle, "setTitle:", NSString.Create(Title ?? string.Empty));
            ObjC.Call(dialog.Handle, "setCanCreateDirectories:", true);

            int result = dialog.Run(window);
            var mappedResult = MapResult(result);
            BeforeReturn(dialog, mappedResult);

            return mappedResult;
        }

        protected abstract NSDialog CreateDialog();

        protected virtual void BeforeShow(NSDialog dialog)
        {
        }

        protected virtual void BeforeReturn(NSDialog dialog, DialogResult result)
        {
        }

        private static DialogResult MapResult(int result)
        {
            return result switch
            {
                1 => DialogResult.Ok,
                0 => DialogResult.Cancel,
                _ => DialogResult.None,
            };
        }
    }
}
