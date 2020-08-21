namespace SpiderEye.Windows
{
    /// <summary>
    /// Defines the different types of webviews on Windows.
    /// </summary>
    public enum WebviewType
    {
        /// <summary>
        /// Latest version available.
        /// </summary>
        Latest,

        /// <summary>
        /// Internet Explorer.
        /// </summary>
        InternetExplorer,

        /// <summary>
        /// Edge based on EdgeHTML and Chakra.
        /// </summary>
        Edge,

        /// <summary>
        /// Edge based on Chromium.
        /// </summary>
        EdgeChromium,
    }
}
