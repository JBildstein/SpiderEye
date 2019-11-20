using System;

namespace SpiderEye.UI
{
    /// <summary>
    /// Describes modifier keys.
    /// </summary>
    [Flags]
    public enum ModifierKey
    {
        /// <summary>
        /// No key.
        /// </summary>
        None = 0,

        /// <summary>
        /// Shift key.
        /// </summary>
        Shift = 1 << 0,

        /// <summary>
        /// Control key.
        /// </summary>
        Control = 1 << 1,

        /// <summary>
        /// Alt/Option key.
        /// </summary>
        Alt = 1 << 2,

        /// <summary>
        /// Super key (e.g. Windows or Command key).
        /// </summary>
        Super = 1 << 3,

        /// <summary>
        /// Control key on Windows and Linux, Command key on macOS.
        /// Using this key makes it easier to define a shortcut that is appropriate for all platforms.
        /// </summary>
        Primary = 1 << 4,
    }
}
