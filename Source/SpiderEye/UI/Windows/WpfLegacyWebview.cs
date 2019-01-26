using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using SpiderEye.Bridge;
using SpiderEye.Configuration;
using SpiderEye.Tools;
using SpiderEye.UI.Windows.Internal;

namespace SpiderEye.UI.Windows
{
    internal class WpfLegacyWebview : IWebview, IWpfWebview
    {
        public event EventHandler PageLoaded;

        public object Control
        {
            get { return webview; }
        }

        private readonly WebBrowser webview;
        private readonly ScriptInterface scriptInterface;
        private readonly string hostAddress;

        public WpfLegacyWebview(AppConfiguration config, string hostAddress, WebviewBridge scriptingApi)
        {
            if (config == null) { throw new ArgumentNullException(nameof(config)); }
            if (scriptingApi == null) { throw new ArgumentNullException(nameof(scriptingApi)); }

            this.hostAddress = hostAddress ?? throw new ArgumentNullException(nameof(hostAddress));

            webview = new WebBrowser();

            if (config.EnableScriptInterface)
            {
                scriptInterface = new ScriptInterface(scriptingApi);
                webview.ObjectForScripting = scriptInterface;
                webview.LoadCompleted += Webview_LoadCompleted;
            }
        }

        public void NavigateToFile(string url)
        {
            var uri = UriTools.Combine(hostAddress, url);
            webview.Navigate(uri);
        }

        public string ExecuteScript(string script)
        {
            return webview.InvokeScript("eval", new string[] { script })?.ToString();
        }

        public Task<string> ExecuteScriptAsync(string script)
        {
            string result = webview.InvokeScript("eval", new string[] { script })?.ToString();
            return Task.FromResult(result);
        }

        public void Dispose()
        {
            webview.Dispose();
        }

        private void Webview_LoadCompleted(object sender, NavigationEventArgs e)
        {
            string initScript = Resources.GetInitScript("Windows");
            ExecuteScript(initScript);

            PageLoaded?.Invoke(this, EventArgs.Empty);
        }
    }
}
