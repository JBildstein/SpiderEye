namespace SpiderEye
{
    /// <summary>
    /// Represents a separator in a menu.
    /// </summary>
    public sealed class SeparatorMenuItem : MenuItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SeparatorMenuItem"/> class.
        /// </summary>
        public SeparatorMenuItem()
            : base(Application.Factory.CreateMenuSeparator())
        {
        }
    }
}
