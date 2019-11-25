using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpiderEye.Bridge;
using SpiderEye.Tools;
using SpiderEye.UI.Windows.Internal;

namespace SpiderEye.UI.Windows
{
    internal class WinFormsLegacyWebview : IWebview, IWinFormsWebview
    {
        public event PageLoadEventHandler PageLoaded;

        public Control Control
        {
            get { return webview; }
        }

        private readonly WebBrowser webview;
        private readonly WindowConfiguration config;
        private readonly ScriptInterface scriptInterface;
        private readonly string hostAddress;

        public WinFormsLegacyWebview(WindowConfiguration config, string hostAddress, WebviewBridge scriptingApi)
        {
            if (scriptingApi == null) { throw new ArgumentNullException(nameof(scriptingApi)); }

            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.hostAddress = hostAddress ?? throw new ArgumentNullException(nameof(hostAddress));

            webview = new WebBrowser
            {
                IsWebBrowserContextMenuEnabled = false,
                TabStop = false,
                AllowNavigation = true,
                AllowWebBrowserDrop = false,
                ScriptErrorsSuppressed = true,
                WebBrowserShortcutsEnabled = false,
            };

            if (config.EnableScriptInterface)
            {
                scriptInterface = new ScriptInterface(scriptingApi);
                webview.ObjectForScripting = scriptInterface;
            }

            webview.DocumentCompleted += Webview_DocumentCompleted;
        }

        public void NavigateToFile(string url)
        {
            var uri = UriTools.Combine(hostAddress, url);
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

        private async void Webview_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webview.ReadyState == WebBrowserReadyState.Complete)
            {
                if (config.EnableScriptInterface)
                {
                    string initScript = Resources.GetInitScript("Windows");
                    await ExecuteScriptAsync(initScript);
                }

                // TODO: figure out how to get success state
                // it may require some ActiveX Voodoo: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.webbrowser.createsink
                PageLoaded?.Invoke(this, PageLoadEventArgs.Successful);
            }
        }
    }
}
