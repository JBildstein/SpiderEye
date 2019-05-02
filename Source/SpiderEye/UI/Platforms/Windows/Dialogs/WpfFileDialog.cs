using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace SpiderEye.UI.Windows.Dialogs
{
    internal abstract class WpfFileDialog : IFileDialog
    {
        public string Title { get; set; }
        public string InitialDirectory { get; set; }
        public string FileName { get; set; }
        public ICollection<FileFilter> FileFilters { get; }

        protected WpfFileDialog()
        {
            FileFilters = new List<FileFilter>();
        }

        public DialogResult Show()
        {
            return Show(null);
        }

        public DialogResult Show(IWindow parent)
        {
            var window = parent as WpfWindow;
            if (parent != null && window == null)
            {
                throw new ArgumentException("Invalid window type.", nameof(parent));
            }

            var dialog = GetDialog();
            dialog.Title = Title;
            dialog.InitialDirectory = InitialDirectory;
            dialog.FileName = FileName;
            dialog.Filter = GetFileFilter(FileFilters);

            bool? result = dialog.ShowDialog(window);
            FileName = dialog.FileName;

            BeforeReturn(dialog);

            if (result == null) { return DialogResult.None; }
            return result.Value ? DialogResult.Ok : DialogResult.Cancel;
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
