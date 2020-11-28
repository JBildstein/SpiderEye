using System;
using System.Collections.Generic;
using System.Linq;
using SpiderEye.Mac.Interop;
using SpiderEye.Mac.Native;

namespace SpiderEye.Mac
{
    internal abstract class CocoaFileDialog : CocoaDialog, IFileDialog
    {
        public string? InitialDirectory { get; set; }
        public string? FileName { get; set; }
        public ICollection<FileFilter> FileFilters { get; }

        protected CocoaFileDialog()
        {
            FileFilters = new List<FileFilter>();
        }

        protected override void BeforeShow(NSDialog dialog)
        {
            if (!string.IsNullOrWhiteSpace(InitialDirectory))
            {
                var url = Foundation.Call("NSURL", "fileURLWithPath:", NSString.Create(InitialDirectory));
                ObjC.Call(dialog.Handle, "setDirectoryURL:", url);
            }

            if (!string.IsNullOrWhiteSpace(FileName))
            {
                ObjC.Call(dialog.Handle, "setNameFieldStringValue:", NSString.Create(FileName));
            }

            SetFileFilters(dialog.Handle, FileFilters);
        }

        protected override void BeforeReturn(NSDialog dialog, DialogResult result)
        {
            if (result == DialogResult.Ok)
            {
                var selection = ObjC.Call(dialog.Handle, "URL");
                FileName = NSString.GetString(ObjC.Call(selection, "path"));
            }
            else { FileName = null; }
        }

        private void SetFileFilters(IntPtr dialog, IEnumerable<FileFilter> filters)
        {
            var fileTypes = filters
                .SelectMany(t => t.Filters.Select(u => u.TrimStart('*', '.')))
                .Distinct()
                .Select(t => NSString.Create(t));

            if (fileTypes.Any())
            {
                var data = fileTypes.ToArray();
                var array = Foundation.Call("NSArray", "arrayWithObjects:count:", data, new IntPtr(data.Length));
                ObjC.Call(dialog, "setAllowedFileTypes:", array);
            }
        }
    }
}
