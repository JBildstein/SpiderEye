using System.Threading.Tasks;
using System.Windows.Controls;
using SpiderEye.Scripting;
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

        public WpfLegacyWebview(bool enableScriptInterface)
        {
            webview = new WebBrowser();

            if (enableScriptInterface)
            {
                ScriptHandler = new ScriptHandler(this);
                scriptInterface = new ScriptInterface(ScriptHandler);
                webview.ObjectForScripting = scriptInterface;
                string initScript = Resources.GetInitScript("Windows");
                webview.LoadCompleted += (s, e) => ExecuteScript(initScript);
            }
        }

        public void LoadUrl(string url)
        {
            webview.Navigate(url);
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
