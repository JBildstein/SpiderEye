using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpiderEye.Bridge;
using SpiderEye.Windows.Internal;

namespace SpiderEye.Windows
{
    /// <summary>
    /// Internet Explorer based webview.
    /// </summary>
    internal class IeLegacyWebview : IWebview, IWinFormsWebview
    {
        public event NavigatingEventHandler Navigating;
        public event PageLoadEventHandler PageLoaded;

        public Control Control
        {
            get { return webview; }
        }

        public bool EnableScriptInterface
        {
            get { return webview.ObjectForScripting == null; }
            set { webview.ObjectForScripting = value ? scriptInterface : null; }
        }

        public bool EnableDevTools { get; set; }

        private readonly WebBrowser webview;
        private readonly ScriptInterface scriptInterface;
        private readonly Uri hostAddress;

        public IeLegacyWebview(string hostAddress, WebviewBridge bridge)
        {
            if (hostAddress == null) { throw new ArgumentNullException(nameof(hostAddress)); }
            if (bridge == null) { throw new ArgumentNullException(nameof(bridge)); }

            this.hostAddress = new Uri(hostAddress, UriKind.Absolute);

            webview = new WebBrowser
            {
                IsWebBrowserContextMenuEnabled = false,
                TabStop = false,
                AllowNavigation = true,
                AllowWebBrowserDrop = false,
                ScriptErrorsSuppressed = true,
                WebBrowserShortcutsEnabled = false,
            };

            scriptInterface = new ScriptInterface(bridge);
            webview.Navigating += Webview_Navigating;
            webview.DocumentCompleted += Webview_DocumentCompleted;
        }

        public void UpdateBackgroundColor(byte r, byte g, byte b)
        {
            // TODO: can't set background color of webview, need to use different strategy to remove any flicker
        }

        public void LoadUri(Uri uri)
        {
            if (uri == null) { throw new ArgumentNullException(nameof(uri)); }

            if (!uri.IsAbsoluteUri) { uri = new Uri(hostAddress, uri); }

            webview.Navigate(uri);
        }

        public Task<string> ExecuteScriptAsync(string script)
        {
            string result = webview.Document.InvokeScript("eval", new string[] { script })?.ToString();
            return Task.FromResult(result);
        }

        public void Dispose()
        {
            webview.Dispose();
        }

        private void Webview_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            var args = new NavigatingEventArgs(e.Url);
            Navigating?.Invoke(this, args);
            e.Cancel = args.Cancel;
        }

        private async void Webview_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webview.ReadyState == WebBrowserReadyState.Complete)
            {
                string initScript = Resources.GetInitScript("Windows");
                await ExecuteScriptAsync(initScript);

                // TODO: figure out how to get success state
                // it may require some ActiveX Voodoo: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.webbrowser.createsink
                PageLoaded?.Invoke(this, new PageLoadEventArgs(e.Url, true));
            }
        }
    }
}
