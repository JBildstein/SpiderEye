using System;
using System.Text;

namespace SpiderEye.Linux
{
    internal class GtkWebview : IWebview
    {
        public event EventHandler CloseRequested;
        public event EventHandler<string> TitleChanged;
        public event EventHandler<string> ScriptInvoked;

        public readonly IntPtr Handle;

        private readonly IntPtr manager;
        private readonly AppConfiguration config;
        private readonly string initScript;

        public GtkWebview(AppConfiguration config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            initScript = Scripts.GetScript("Linux", "InitScript.js");
            manager = WebKit.Manager.Create();
            using (GLibString name = "script-message-received::external")
            {
                GLib.ConnectSignal(manager, name, (ScriptDelegate)ScriptCallback, IntPtr.Zero);
            }

            using (GLibString name = "external")
            {
                WebKit.Manager.RegisterScriptMessageHandler(manager, name);
            }

            Handle = WebKit.CreateWithUserContentManager(manager);
            using (GLibString name = "load-changed")
            {
                GLib.ConnectSignal(Handle, name, (PageLoadDelegate)LoadCallback, IntPtr.Zero);
            }

            using (GLibString name = "context-menu")
            {
                GLib.ConnectSignal(Handle, name, (ContextMenuRequestDelegate)ContextMenuCallback, IntPtr.Zero);
            }

            using (GLibString name = "notify::title")
            {
                GLib.ConnectSignal(Handle, name, (WebviewDelegate)TitleChangeCallback, IntPtr.Zero);
            }

            using (GLibString name = "close")
            {
                GLib.ConnectSignal(Handle, name, (WebviewDelegate)CloseCallback, IntPtr.Zero);
            }
        }

        public void LoadUrl(string url)
        {
            using (GLibString gurl = url)
            {
                WebKit.LoadUri(Handle, gurl);
            }
        }

        public void ExecuteScript(string script)
        {
            using (GLibString gscript = script)
            {
                WebKit.RunJavaScript(Handle, gscript, IntPtr.Zero, null, IntPtr.Zero);
            }
        }

        private unsafe void ScriptCallback(IntPtr manager, IntPtr jsResult, IntPtr userdata)
        {
            IntPtr value = WebKit.JavaScript.GetValue(jsResult);
            if (WebKit.JavaScript.IsValueString(value))
            {
                IntPtr bytes = WebKit.JavaScript.GetStringBytes(value);
                IntPtr bytesPtr = GLib.GetBytesDataPointer(bytes, out UIntPtr length);

                string result = Encoding.UTF8.GetString((byte*)bytesPtr, (int)length);
                ScriptInvoked?.Invoke(this, result);

                GLib.UnrefBytes(bytes);
            }
        }

        private void LoadCallback(IntPtr webview, WebKitLoadEvent type, IntPtr userdata)
        {
            if (type == WebKitLoadEvent.Finished)
            {
                ExecuteScript(initScript);
            }
        }

        private bool ContextMenuCallback(IntPtr webview, IntPtr default_menu, IntPtr hit_test_result, bool triggered_with_keyboard, IntPtr arg)
        {
            // this simply prevents the default context menu from showing up
            return true;
        }

        private void CloseCallback(IntPtr webview, IntPtr userdata)
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        private void TitleChangeCallback(IntPtr webview, IntPtr userdata)
        {
            string title = GLibString.FromPointer(WebKit.GetTitle(webview));
            TitleChanged?.Invoke(this, title);
        }
    }
}
