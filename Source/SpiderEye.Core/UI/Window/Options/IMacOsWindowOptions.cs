namespace SpiderEye
{
    /// <summary>
    /// Accessor for mac os related window options.
    /// </summary>
    public interface IMacOsWindowOptions
    {
        /// <summary>
        /// Gets or sets the mac os appearance.
        /// </summary>
        MacOsAppearance? Appearance { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use a transparent title bar.
        /// </summary>
        bool TransparentTitleBar { get; set; }
    }
}
