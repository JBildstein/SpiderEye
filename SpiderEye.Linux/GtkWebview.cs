using System;
using System.IO;
using System.Text;
using SpiderEye.Tools;

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

        public GtkWebview(AppConfiguration config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

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

            using (GLibString name = "notify::title")
            {
                GLib.ConnectSignal(Handle, name, (WebviewDelegate)TitleChangeCallback, IntPtr.Zero);
            }

            using (GLibString name = "close")
            {
                GLib.ConnectSignal(Handle, name, (WebviewDelegate)CloseCallback, IntPtr.Zero);
            }

            if (config.ShowDevTools)
            {
                IntPtr settings = WebKit.Settings.Get(Handle);
                WebKit.Settings.SetEnableWriteConsoleMessagesToStdout(settings, true);
                WebKit.Settings.SetEnableDeveloperExtras(settings, true);

                IntPtr inspector = WebKit.Inspector.Get(Handle);
                WebKit.Inspector.Show(inspector);
            }

            IntPtr context = WebKit.Context.GetDefault();
            using (GLibString name = "spider")
            {
                WebKit.Context.RegisterUriScheme(context, name, FileRequestCallback, IntPtr.Zero, IntPtr.Zero);
            }
        }

        public void LoadUrl(string url)
        {
            using (GLibString gurl = url)
            {
                WebKit.LoadUri(Handle, gurl);
            }
        }

        public void RunJs(string script)
        {
            using (GLibString gscript = script)
            {
                WebKit.RunJavaScript(Handle, gscript, IntPtr.Zero, null, IntPtr.Zero);
            }
        }

        private void InsertExternHandler()
        {
            const string script = "window.external={invoke:function(x){" +
                "window.webkit.messageHandlers.external.postMessage(x);}}";
            RunJs(script);
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
                InsertExternHandler();
            }
        }

        private bool ContextMenuCallback(IntPtr webview, IntPtr default_menu, IntPtr hit_test_result, bool triggered_with_keyboard, IntPtr arg)
        {
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

        private unsafe void FileRequestCallback(IntPtr request, IntPtr userdata)
        {
            string uri = GLibString.FromPointer(WebKit.UriScheme.GetRequestUri(request));
            using (var resourceStream = ResourceUriResolver.Instance.GetResource(uri))
            {
                if (resourceStream != null)
                {
                    using (var reader = new StreamReader(resourceStream))
                    {
                        string dataString = reader.ReadToEnd();
                        byte[] data = Encoding.UTF8.GetBytes(dataString);
                        fixed (byte* ptr = data)
                        {
                            IntPtr stream = GLib.CreateStreamFromData((IntPtr)ptr, data.Length, IntPtr.Zero);
                            WebKit.UriScheme.FinishSchemeRequest(request, stream, data.Length, IntPtr.Zero);
                            GLib.UnrefObject(stream);
                        }
                    }
                }
                else
                {
                    var error = new GError(
                        domain: GLib.GetFileErrorQuark(),
                        code: (int)GFileError.NOENT,
                        message: IntPtr.Zero);
                    WebKit.UriScheme.FinishSchemeRequestWithError(request, ref error);
                }
            }
        }
    }
}
