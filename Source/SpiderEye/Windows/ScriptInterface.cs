using System;
using System.Runtime.InteropServices;

namespace SpiderEye.Windows.Internal
{
    /// <summary>
    /// Script interface for the WPF WebBrowser control.
    /// Not meant for public consumption.
    /// </summary>
    [ComVisible(true)]
    public class ScriptInterface
    {
        internal event EventHandler<string> TitleChanged;

        internal ScriptInterface()
        {
        }

        /// <summary>
        /// Title change hook.
        /// </summary>
        /// <param name="title">The changed title.</param>
        public void ChangeTitle(string title)
        {
            TitleChanged?.Invoke(this, title);
        }
    }
}
