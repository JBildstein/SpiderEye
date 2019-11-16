using System.Windows.Forms;

namespace SpiderEye.UI.Windows
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
    }
}
