using System;

namespace SpiderEye
{
    /// <summary>
    /// Represents an item in a menu with a label.
    /// </summary>
    public class LabelMenuItem : MenuItem
    {
        /// <summary>
        /// Fires when the menu item is clicked on or otherwise activated.
        /// </summary>
        public event EventHandler Click
        {
            add { NativeLabelMenuItem.Click += value; }
            remove { NativeLabelMenuItem.Click -= value; }
        }

        /// <summary>
        /// Gets or sets the label of this menu.
        /// </summary>
        public string Label
        {
            get { return NativeLabelMenuItem.Label; }
            set { NativeLabelMenuItem.Label = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the menu item is enabled or not.
        /// </summary>
        public bool Enabled
        {
            get { return NativeLabelMenuItem.Enabled; }
            set { NativeLabelMenuItem.Enabled = value; }
        }

        /// <summary>
        /// Gets the native menu item.
        /// </summary>
        internal ILabelMenuItem NativeLabelMenuItem { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LabelMenuItem"/> class.
        /// </summary>
        public LabelMenuItem()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LabelMenuItem"/> class.
        /// </summary>
        /// <param name="label">Gets or sets the label of this menu item.</param>
        public LabelMenuItem(string label)
            : this(Application.Factory.CreateLabelMenu(label))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LabelMenuItem"/> class.
        /// </summary>
        /// <param name="nativeMenu">Gets or sets the native menu item.</param>
        internal LabelMenuItem(ILabelMenuItem nativeMenu)
            : base(nativeMenu)
        {
            NativeLabelMenuItem = nativeMenu;
        }

        /// <summary>
        /// Sets a keyboard shortcut to the menu item.
        /// </summary>
        /// <param name="modifier">The shortcut modifier key.</param>
        /// <param name="key">The shortcut key.</param>
        public void SetShortcut(ModifierKey modifier, Key key)
        {
            NativeLabelMenuItem.SetShortcut(modifier, key);
        }
    }
}
