using System;
using System.Collections.Generic;

namespace SpiderEye.UI.Mac.Dialogs
{
    internal class CocoaOpenFileDialog : IOpenFileDialog
    {
        public string Title { get; set; }
        public string InitialDirectory { get; set; }
        public string FileName { get; set; }
        public bool Multiselect { get; set; }
        public ICollection<FileFilter> FileFilters { get; }
        public string[] SelectedFiles
        {
            get;
            private set;
        }

        public DialogResult Show()
        {
            throw new NotImplementedException();
        }

        public DialogResult Show(IWindow parent)
        {
            throw new NotImplementedException();
        }
    }
}
