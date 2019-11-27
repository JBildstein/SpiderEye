namespace SpiderEye.Bridge.Models
{
    internal class FileFilterModel
    {
        public string Name { get; set; }
        public string[] Filters { get; set; }

        public FileFilter ToFilter()
        {
            return new FileFilter(Name ?? string.Empty, Filters);
        }
    }
}
