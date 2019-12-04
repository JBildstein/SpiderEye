using System;
using SpiderEye.Mac.Interop;
using SpiderEye.Mac.Native;

namespace SpiderEye.Mac
{
    internal class CocoaOpenFileDialog : CocoaFileDialog, IOpenFileDialog
    {
        public bool Multiselect { get; set; }

        public string[] SelectedFiles
        {
            get;
            private set;
        }

        protected override NSDialog CreateDialog()
        {
            var panel = NSDialog.CreateOpenPanel();

            ObjC.Call(panel.Handle, "setCanChooseFiles:", true);
            ObjC.Call(panel.Handle, "setCanChooseDirectories:", false);
            ObjC.Call(panel.Handle, "setAllowsMultipleSelection:", Multiselect);

            return panel;
        }

        protected override void BeforeReturn(NSDialog dialog, DialogResult result)
        {
            base.BeforeReturn(dialog, result);

            var urls = ObjC.Call(dialog.Handle, "URLs");
            int count = ObjC.Call(urls, "count").ToInt32();
            string[] files = new string[count];
            for (int i = 0; i < count; i++)
            {
                var url = ObjC.Call(urls, "objectAtIndex:", new IntPtr(i));
                files[i] = NSString.GetString(ObjC.Call(url, "path"));
            }

            SelectedFiles = files;
        }
    }
}
