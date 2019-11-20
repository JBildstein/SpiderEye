using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SpiderEye.UI.Windows.Interop;

namespace SpiderEye.UI.Windows.Dialogs
{
    internal abstract class WinFormsFileDialog
    {
        public string Title { get; set; }
        public string InitialDirectory { get; set; }
        public string FileName { get; set; }
        public ICollection<FileFilter> FileFilters { get; }

        protected WinFormsFileDialog()
        {
            FileFilters = new List<FileFilter>();
        }

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

            var dialog = GetDialog();
            dialog.Title = Title;
            dialog.InitialDirectory = InitialDirectory;
            dialog.FileName = FileName;
            dialog.Filter = GetFileFilter(FileFilters);

            var result = dialog.ShowDialog(window);
            FileName = dialog.FileName;

            BeforeReturn(dialog);

            return WinFormsMapper.MapResult(result);
        }

        protected abstract FileDialog GetDialog();

        protected virtual void BeforeReturn(FileDialog dialog)
        {
        }

        private string GetFileFilter(IEnumerable<FileFilter> filters)
        {
            if (!filters.Any()) { return null; }

            var builder = new StringBuilder();
            bool first = true;
            foreach (var filter in filters)
            {
                if (!first) { builder.Append('|'); }
                first = false;

                builder.Append(filter.Name);
                builder.Append('|');

                for (int i = 0; i < filter.Filters.Length; i++)
                {
                    if (i > 0) { builder.Append(';'); }

                    builder.Append(filter.Filters[i]);
                }
            }

            return builder.ToString();
        }
    }
}
