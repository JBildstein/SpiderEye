using SpiderEye.Mac.Interop;
using SpiderEye.Mac.Native;

namespace SpiderEye.Mac
{
    internal class CocoaFolderSelectDialog : CocoaDialog, IFolderSelectDialog
    {
        public string SelectedPath { get; set; }

        protected override NSDialog CreateDialog()
        {
            var panel = NSDialog.CreateOpenPanel();

            ObjC.Call(panel.Handle, "setCanChooseFiles:", false);
            ObjC.Call(panel.Handle, "setCanChooseDirectories:", true);
            ObjC.Call(panel.Handle, "setAllowsMultipleSelection:", false);

            if (!string.IsNullOrWhiteSpace(SelectedPath))
            {
                var url = Foundation.Call("NSURL", "fileURLWithPath:", NSString.Create(SelectedPath));
                ObjC.Call(panel.Handle, "setDirectoryURL:", url);
            }

            return panel;
        }

        protected override void BeforeReturn(NSDialog dialog, DialogResult result)
        {
            if (result == DialogResult.Ok)
            {
                var selection = ObjC.Call(dialog.Handle, "URL");
                SelectedPath = NSString.GetString(ObjC.Call(selection, "path"));
            }
            else { SelectedPath = null; }
        }
    }
}
