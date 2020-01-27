using System;
using System.Collections;
using System.Collections.Generic;

namespace SpiderEye
{
    /// <summary>
    /// Represents a collection of menu items.
    /// </summary>
    public sealed class MenuItemCollection : IEnumerable<MenuItem>, IReadOnlyCollection<MenuItem>
    {
        /// <inheritdoc/>
        public int Count
        {
            get { return menuItems.Count; }
        }

        private readonly List<MenuItem> menuItems = new List<MenuItem>();
        private readonly IMenu menu;

        internal MenuItemCollection(IMenu menu)
        {
            this.menu = menu;
        }

        /// <summary>
        /// Adds a new menu item to this menu.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(MenuItem item)
        {
            if (item == null) { throw new ArgumentNullException(nameof(item)); }

            menu.AddItem(item.NativeMenuItem);
            menuItems.Add(item);
        }

        /// <summary>
        /// Adds a new label menu item and returns it.
        /// </summary>
        /// <param name="label">The label text.</param>
        /// <returns>The created menu item.</returns>
        public LabelMenuItem AddLabelItem(string label)
        {
            var item = new LabelMenuItem(label);
            Add(item);
            return item;
        }

        /// <summary>
        /// Adds a new separator menu item.
        /// </summary>
        public void AddSeparatorItem()
        {
            Add(new SeparatorMenuItem());
        }

        /// <inheritdoc/>
        public IEnumerator<MenuItem> GetEnumerator()
        {
            return menuItems.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return menuItems.GetEnumerator();
        }
    }
}
