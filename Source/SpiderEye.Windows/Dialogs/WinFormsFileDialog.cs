using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpiderEye.Tools;
using SpiderEye.Windows.Interop;
using WFFileDialog = System.Windows.Forms.FileDialog;

namespace SpiderEye.Windows
{
    internal abstract class WinFormsFileDialog : IFileDialog
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
            var dialog = GetDialog();
            dialog.Title = Title;
            dialog.InitialDirectory = InitialDirectory;
            dialog.FileName = FileName;
            dialog.Filter = GetFileFilter(FileFilters);

            var window = NativeCast.To<WinFormsWindow>(parent);
            var result = dialog.ShowDialog(window);
            FileName = dialog.FileName;

            BeforeReturn(dialog);

            return WinFormsMapper.MapResult(result);
        }

        protected abstract WFFileDialog GetDialog();

        protected virtual void BeforeReturn(WFFileDialog dialog)
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
