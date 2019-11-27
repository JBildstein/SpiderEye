namespace SpiderEye.Mac.Interop
{
    /// <summary>
    /// Image scaling types.
    /// </summary>
    internal enum NSImageScaling
    {
        /// <summary>
        /// If it is too large for the destination, scale the image
        /// down while preserving the aspect ratio.
        /// </summary>
        ProportionallyDown = 0,

        /// <summary>
        /// Scale each dimension to exactly fit destination.
        /// </summary>
        AxesIndependently = 1,

        /// <summary>
        /// Do not scale the image.
        /// </summary>
        None = 2,

        /// <summary>
        /// Scale the image to its maximum possible dimensions while both
        /// staying within the destination area and preserving its aspect ratio.
        /// </summary>
        ProportionallyUpOrDown = 3,
    }
}
