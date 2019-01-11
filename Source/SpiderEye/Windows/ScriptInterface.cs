#pragma warning disable SA1300, IDE1006 // Element should begin with upper-case letter

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using SpiderEye.Tools.Scripting;

namespace SpiderEye.Windows.Internal
{
    /// <summary>
    /// Script interface for the WPF WebBrowser control.
    /// Not meant for public consumption.
    /// </summary>
    [ComVisible(true)]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ScriptInterface
    {
        private readonly ScriptHandler handler;

        internal ScriptInterface(ScriptHandler handler)
        {
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        /// <summary>
        /// Script invoke hook.
        /// </summary>
        /// <param name="data">The sent data.</param>
        public void notify(string data)
        {
            handler.HandleScriptCall(data);
        }
    }
}
