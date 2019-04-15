using System;
using System.Threading.Tasks;

namespace SpiderEye.UI
{
    /// <summary>
    /// Represents a status icon.
    /// </summary>
    public interface IStatusIcon : IDisposable
    {
        /// <summary>
        /// Gets or sets the status icon title.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        Icon Icon { get; set; }

        /// <summary>
        /// Adds a menu to the status icon and returns it.
        /// </summary>
        /// <returns>The crated menu.</returns>
        IMenu AddMenu();
    }
}
