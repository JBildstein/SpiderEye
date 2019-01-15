using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using SpiderEye.Scripting;
using SpiderEye.UI.Windows.Internal;

namespace SpiderEye.UI.Windows
{
    internal class WpfLegacyWebview : IWebview, IWpfWebview
    {
        public ScriptHandler ScriptHandler { get; }

        public IDisposable Control
        {
            get { return webview; }
        }

        private readonly AppConfiguration config;
        private readonly WebBrowser webview;
        private readonly ScriptInterface scriptInterface;
        private readonly string initScript;

        public WpfLegacyWebview(AppConfiguration config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            webview = new WebBrowser();
            ScriptHandler = new ScriptHandler(this);
            scriptInterface = new ScriptInterface(ScriptHandler);
            initScript = Resources.GetInitScript("Windows");
            webview.ObjectForScripting = scriptInterface;
            webview.LoadCompleted += Webview_LoadCompleted;
        }

        public void LoadUrl(string url)
        {
            webview.Navigate(url);
        }

        public void ExecuteScript(string script)
        {
            webview.InvokeScript("eval", new string[] { script });
        }

        public Task<string> CallFunction(string function)
        {
            string result = webview.InvokeScript("eval", new string[] { function }) as string;
            return Task.FromResult(result);
        }

        private void Webview_LoadCompleted(object sender, NavigationEventArgs e)
        {
            ExecuteScript(initScript);
        }
    }
}
