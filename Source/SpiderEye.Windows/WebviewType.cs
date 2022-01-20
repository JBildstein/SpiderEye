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
        Latest = 0,

        /// <summary>
        /// Internet Explorer.
        /// </summary>
        InternetExplorer = 1,

        // Edge based on EdgeHTML and Chakra used to be 2

        /// <summary>
        /// Edge based on Chromium.
        /// </summary>
        EdgeChromium = 3,
    }
}
