using System.Threading.Tasks;
using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;
using Microsoft.Toolkit.Wpf.UI.Controls;
using SpiderEye.Scripting;

namespace SpiderEye.UI.Windows
{
    internal class WpfWebview : IWebview, IWpfWebview
    {
        public ScriptHandler ScriptHandler { get; }

        public object Control
        {
            get { return webview; }
        }

        private readonly WebView webview;

        public WpfWebview(bool enableScriptInterface)
        {
            webview = new WebView();

            webview.IsScriptNotifyAllowed = enableScriptInterface;
            if (enableScriptInterface)
            {
                ScriptHandler = new ScriptHandler(this);
                webview.ScriptNotify += Webview_ScriptNotify;
                webview.AddInitializeScript(Resources.GetInitScript("Windows"));
            }
        }

        public void LoadUrl(string url)
        {
            webview.Navigate(url);
        }

        public string ExecuteScript(string script)
        {
            return webview.InvokeScript("eval", new string[] { script });
        }

        public async Task<string> ExecuteScriptAsync(string script)
        {
            return await webview.InvokeScriptAsync("eval", new string[] { script });
        }

        public void Dispose()
        {
            webview.Dispose();
        }

        private void Webview_ScriptNotify(object sender, WebViewControlScriptNotifyEventArgs e)
        {
            ScriptHandler.HandleScriptCall(e.Value);
        }
    }
}
