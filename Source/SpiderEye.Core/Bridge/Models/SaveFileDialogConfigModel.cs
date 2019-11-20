using System.Collections.Generic;

namespace SpiderEye.Bridge.Models
{
    internal class SaveFileDialogConfigModel
    {
        public string Title { get; set; }
        public string InitialDirectory { get; set; }
        public string FileName { get; set; }
        public IEnumerable<FileFilterModel> FileFilters { get; set; }
        public bool OverwritePrompt { get; set; }
    }
}
