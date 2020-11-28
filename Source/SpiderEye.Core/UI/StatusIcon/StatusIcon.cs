using System;
using System.Reflection;

namespace SpiderEye
{
    /// <summary>
    /// Represents a status icon.
    /// </summary>
    public sealed class StatusIcon : IDisposable
    {
        /// <summary>
        /// Gets or sets the status icon title.
        /// </summary>
        public string? Title
        {
            get { return NativeStatusIcon.Title; }
            set { NativeStatusIcon.Title = value; }
        }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        public AppIcon? Icon
        {
            get { return NativeStatusIcon.Icon; }
            set { NativeStatusIcon.Icon = value; }
        }

        /// <summary>
        /// Gets or sets the icon menu.
        /// </summary>
        public Menu? Menu
        {
            get { return NativeStatusIcon.Menu; }
            set { NativeStatusIcon.Menu = value; }
        }

        /// <summary>
        /// Gets the native status icon.
        /// </summary>
        internal IStatusIcon NativeStatusIcon { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusIcon"/> class.
        /// </summary>
        public StatusIcon()
        {
            string title = Assembly.GetEntryAssembly()?.GetName().Name ?? string.Empty;
            NativeStatusIcon = Application.Factory.CreateStatusIcon(title);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            NativeStatusIcon.Dispose();
        }
    }
}
