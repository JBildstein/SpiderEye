#pragma warning disable SA1300, IDE1006 // Element should begin with upper-case letter

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using SpiderEye.Bridge;

namespace SpiderEye.UI.Windows.Internal
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
        private readonly WebviewBridge bridge;

        internal ScriptInterface(WebviewBridge bridge)
        {
            this.bridge = bridge ?? throw new ArgumentNullException(nameof(bridge));
        }

        /// <summary>
        /// Script invoke hook.
        /// </summary>
        /// <param name="data">The sent data.</param>
        public async void notify(string data)
        {
            await bridge.HandleScriptCall(data);
        }
    }
}
