﻿using System;
using System.Windows.Controls;
using System.Windows.Navigation;
using SpiderEye.Windows.Internal;

namespace SpiderEye.Windows
{
    internal class WpfLegacyWebview : IWebview, IWpfWebview
    {
        public event EventHandler<string> TitleChanged;

        public object Control
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
            scriptInterface = new ScriptInterface();
            initScript = Scripts.GetScript("InitScriptLegacy.js");
            webview.ObjectForScripting = scriptInterface;
            webview.LoadCompleted += Webview_LoadCompleted;
            scriptInterface.TitleChanged += (s, e) => TitleChanged?.Invoke(this, e);
        }

        public void LoadUrl(string url)
        {
            var host = new Uri(config.Host);
            webview.Navigate(new Uri(host, url));
        }

        public void RunJs(string script)
        {
            webview.InvokeScript("eval", new string[] { script });
        }

        private void Webview_LoadCompleted(object sender, NavigationEventArgs e)
        {
            RunJs(initScript);
        }
    }
}