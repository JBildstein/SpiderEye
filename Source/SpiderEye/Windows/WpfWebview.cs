using System;
using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;
using Microsoft.Toolkit.Wpf.UI.Controls;

namespace SpiderEye.Windows
{
    internal class WpfWebview : IWebview, IWpfWebview
    {
        public event EventHandler<string> TitleChanged;

        public object Control
        {
            get { return webview; }
        }

        private readonly AppConfiguration config;
        private readonly WebView webview;

        public WpfWebview(AppConfiguration config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            webview = new WebView();
            webview.IsScriptNotifyAllowed = true;
            webview.ScriptNotify += Webview_ScriptNotify;
            webview.AddInitializeScript(Scripts.GetScript("Windows", "InitScript.js"));
        }

        public void LoadUrl(string url)
        {
            var host = new Uri(config.Host);
            webview.Navigate(new Uri(host, url));
        }

        public void ExecuteScript(string script)
        {
            webview.InvokeScript("eval", new string[] { script });
        }

        private void Webview_ScriptNotify(object sender, WebViewControlScriptNotifyEventArgs e)
        {
            TitleChanged?.Invoke(this, e.Value);
        }
    }
}
