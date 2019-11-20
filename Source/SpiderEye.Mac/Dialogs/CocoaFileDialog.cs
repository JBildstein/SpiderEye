using System;
using System.Collections.Generic;
using System.Linq;
using SpiderEye.UI.Mac.Interop;
using SpiderEye.UI.Mac.Native;

namespace SpiderEye.UI.Mac.Dialogs
{
    internal abstract class CocoaFileDialog : IFileDialog
    {
        public string Title { get; set; }
        public string InitialDirectory { get; set; }
        public string FileName { get; set; }
        public ICollection<FileFilter> FileFilters { get; }

        protected CocoaFileDialog()
        {
            FileFilters = new List<FileFilter>();
        }

        public DialogResult Show()
        {
            return Show(null);
        }

        public DialogResult Show(IWindow parent)
        {
            var window = parent as CocoaWindow;
            if (parent != null && window == null)
            {
                throw new ArgumentException("Invalid window type.", nameof(parent));
            }

            var dialog = CreateDialog();

            if (!string.IsNullOrWhiteSpace(InitialDirectory))
            {
                var url = Foundation.Call("NSURL", "fileURLWithPath:", NSString.Create(InitialDirectory));
                ObjC.Call(dialog.Handle, "setDirectoryURL:", url);
            }

            if (!string.IsNullOrWhiteSpace(FileName))
            {
                ObjC.Call(dialog.Handle, "setNameFieldStringValue:", NSString.Create(FileName));
            }

            ObjC.Call(dialog.Handle, "setTitle:", NSString.Create(Title));
            ObjC.Call(dialog.Handle, "setCanCreateDirectories:", true);
            SetFileFilters(dialog.Handle, FileFilters);

            int result = dialog.Run(window);

            var selection = ObjC.Call(dialog.Handle, "URL");
            FileName = NSString.GetString(ObjC.Call(selection, "path"));

            BeforeReturn(dialog);

            return MapResult(result);
        }

        protected abstract NSDialog CreateDialog();

        protected virtual void BeforeReturn(NSDialog dialog)
        {
        }

        private DialogResult MapResult(int result)
        {
            switch (result)
            {
                case 1:
                    return DialogResult.Ok;

                case 0:
                    return DialogResult.Cancel;

                default:
                    return DialogResult.None;
            }
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
