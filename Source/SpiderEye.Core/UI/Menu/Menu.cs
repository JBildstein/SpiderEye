using System;

namespace SpiderEye
{
    /// <summary>
    /// Represents a menu.
    /// </summary>
    public sealed class Menu : IDisposable
    {
        /// <summary>
        /// Gets the menu sub-items.
        /// </summary>
        public MenuItemCollection MenuItems { get; }

        /// <summary>
        /// Gets the native menu.
        /// </summary>
        internal IMenu NativeMenu { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Menu"/> class.
        /// </summary>
        public Menu()
            : this(Application.Factory.CreateMenu())
        {
        }

        internal Menu(IMenu nativeMenu)
        {
            NativeMenu = nativeMenu;
            MenuItems = new MenuItemCollection(nativeMenu);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            NativeMenu.Dispose();
        }
    }
}
