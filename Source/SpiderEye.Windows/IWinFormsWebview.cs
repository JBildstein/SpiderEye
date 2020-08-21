using System.Windows.Forms;

namespace SpiderEye.Windows
{
    /// <summary>
    /// Represents a Windows Forms webview.
    /// </summary>
    internal interface IWinFormsWebview : IWebview
    {
        /// <summary>
        /// Gets the webview control.
        /// </summary>
        Control Control { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the scripting interface
        /// between browser and window is enabled.
        /// </summary>
        bool EnableScriptInterface { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether dev tools are enabled or not.
        /// </summary>
        bool EnableDevTools { get; set; }

        /// <summary>
        /// Updates the background color of the webview.
        /// </summary>
        /// <param name="r">The red channel.</param>
        /// <param name="g">The green channel.</param>
        /// <param name="b">The blue channel.</param>
        void UpdateBackgroundColor(byte r, byte g, byte b);
    }
}
