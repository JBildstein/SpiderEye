using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderEye.Windows
{
    internal abstract class WinFormsFileDialog<T> : WinFormsDialog<T>, IFileDialog
        where T : System.Windows.Forms.FileDialog
    {
        public string InitialDirectory { get; set; }
        public string FileName { get; set; }
        public ICollection<FileFilter> FileFilters { get; }

        protected WinFormsFileDialog()
        {
            FileFilters = new List<FileFilter>();
        }

        protected override void BeforeShow(T dialog)
        {
            dialog.Title = Title;
            dialog.InitialDirectory = InitialDirectory;
            dialog.FileName = FileName;
            dialog.Filter = GetFileFilter(FileFilters);
        }

        protected override void BeforeReturn(T dialog)
        {
            FileName = dialog.FileName;
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
