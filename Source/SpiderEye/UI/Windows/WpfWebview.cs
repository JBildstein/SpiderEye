using System;
using System.Threading.Tasks;
using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;
using Microsoft.Toolkit.Wpf.UI.Controls;
using SpiderEye.Scripting;

namespace SpiderEye.UI.Windows
{
    internal class WpfWebview : IWebview, IWpfWebview
    {
        public ScriptHandler ScriptHandler { get; }

        public IDisposable Control
        {
            get { return webview; }
        }

        private readonly AppConfiguration config;
        private readonly WebView webview;

        public WpfWebview(AppConfiguration config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            webview = new WebView();
            ScriptHandler = new ScriptHandler(this);
            webview.IsScriptNotifyAllowed = true;
            webview.ScriptNotify += Webview_ScriptNotify;
            webview.AddInitializeScript(Resources.GetInitScript("Windows"));
        }

        public void LoadUrl(string url)
        {
            webview.Navigate(url);
        }

        public void ExecuteScript(string script)
        {
            webview.InvokeScript("eval", new string[] { script });
        }

        public async Task<string> CallFunction(string function)
        {
            return await webview.InvokeScriptAsync("eval", new string[] { function });
        }

        private void Webview_ScriptNotify(object sender, WebViewControlScriptNotifyEventArgs e)
        {
            ScriptHandler.HandleScriptCall(e.Value);
        }
    }
}
