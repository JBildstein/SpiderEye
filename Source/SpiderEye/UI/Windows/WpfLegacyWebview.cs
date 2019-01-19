using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using SpiderEye.Configuration;
using SpiderEye.Scripting;
using SpiderEye.Tools;
using SpiderEye.UI.Windows.Internal;

namespace SpiderEye.UI.Windows
{
    internal class WpfLegacyWebview : IWebview, IWpfWebview
    {
        public ScriptHandler ScriptHandler { get; }

        public object Control
        {
            get { return webview; }
        }

        private readonly WebBrowser webview;
        private readonly ScriptInterface scriptInterface;
        private readonly string hostAddress;

        public WpfLegacyWebview(AppConfiguration config, string hostAddress)
        {
            if (config == null) { throw new ArgumentNullException(nameof(config)); }

            this.hostAddress = hostAddress ?? throw new ArgumentNullException(nameof(hostAddress));

            webview = new WebBrowser();

            if (config.EnableScriptInterface)
            {
                ScriptHandler = new ScriptHandler(this);
                scriptInterface = new ScriptInterface(ScriptHandler);
                webview.ObjectForScripting = scriptInterface;
                string initScript = Resources.GetInitScript("Windows");
                webview.LoadCompleted += (s, e) => ExecuteScript(initScript);
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
    }
}
