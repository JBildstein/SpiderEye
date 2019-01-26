using System;
namespace SpiderEye.UI
{
    /// <summary>
    /// Contains information for filtering files.
    /// </summary>
    public sealed class FileFilter
    {
        /// <summary>
        /// Gets the filter name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the file filters.
        /// </summary>
        public string[] Filters { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileFilter"/> class.
        /// </summary>
        /// <param name="name">The filter name.</param>
        /// <param name="filters">The file filters.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="filters"/> is null.</exception>
        public FileFilter(string name, params string[] filters)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Filters = filters ?? throw new ArgumentNullException(nameof(filters));
        }
    }
}
