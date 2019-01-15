using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SpiderEye.Tools.Scripting;

namespace SpiderEye.Linux
{
    internal class GtkWebview : IWebview
    {
        public event EventHandler CloseRequested;
        public event EventHandler<string> TitleChanged;

        public ScriptHandler ScriptHandler { get; }

        public readonly IntPtr Handle;

        private readonly IntPtr manager;
        private readonly AppConfiguration config;
        private readonly string initScript;

        public GtkWebview(AppConfiguration config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            initScript = Resources.GetInitScript("Linux");
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
                WebKit.JavaScript.BeginExecute(Handle, gscript, IntPtr.Zero, null, IntPtr.Zero);
            }
        }

        public async Task<string> CallFunction(string function)
        {
            return await Task.Run(() =>
            {
                var taskResult = new TaskCompletionSource<string>();

                unsafe void Callback(IntPtr webview, IntPtr asyncResult, IntPtr userdata)
                {
                    IntPtr jsResult = IntPtr.Zero;
                    try
                    {
                        jsResult = WebKit.JavaScript.EndExecute(webview, asyncResult, out IntPtr errorPtr);
                        if (jsResult != IntPtr.Zero)
                        {
                            IntPtr value = WebKit.JavaScript.GetValue(jsResult);
                            if (WebKit.JavaScript.IsValueString(value))
                            {
                                IntPtr bytes = WebKit.JavaScript.GetStringBytes(value);
                                IntPtr bytesPtr = GLib.GetBytesDataPointer(bytes, out UIntPtr length);

                                string result = Encoding.UTF8.GetString((byte*)bytesPtr, (int)length);
                                taskResult.TrySetResult(result);

                                GLib.UnrefBytes(bytes);
                            }
                        }
                        else
                        {
                            try
                            {
                                var error = Marshal.PtrToStructure<GError>(errorPtr);
                                string errorMessage = GLibString.FromPointer(error.Message);
                                taskResult.TrySetException(new Exception($"Script execution failed with {errorMessage}"));
                            }
                            finally { GLib.FreeError(errorPtr); }
                        }
                    }
                    finally
                    {
                        if (jsResult != IntPtr.Zero)
                        {
                            WebKit.JavaScript.ReleaseJsResult(jsResult);
                        }
                    }
                }

                using (GLibString gfunction = function)
                {
                    WebKit.JavaScript.BeginExecute(Handle, gfunction, IntPtr.Zero, Callback, IntPtr.Zero);
                }

                return taskResult.Task;
            });
        }

        private unsafe void ScriptCallback(IntPtr manager, IntPtr jsResult, IntPtr userdata)
        {
            IntPtr value = WebKit.JavaScript.GetValue(jsResult);
            if (WebKit.JavaScript.IsValueString(value))
            {
                IntPtr bytes = IntPtr.Zero;
                try
                {
                    bytes = WebKit.JavaScript.GetStringBytes(value);
                    IntPtr bytesPtr = GLib.GetBytesDataPointer(bytes, out UIntPtr length);

                    string result = Encoding.UTF8.GetString((byte*)bytesPtr, (int)length);
                    ScriptHandler.HandleScriptCall(result);
                }
                finally { if (bytes != IntPtr.Zero) { GLib.UnrefBytes(bytes); } }
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
