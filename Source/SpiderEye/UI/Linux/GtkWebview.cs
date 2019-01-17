using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SpiderEye.Scripting;
using SpiderEye.UI.Linux.Interop;
using SpiderEye.UI.Linux.Native;

namespace SpiderEye.UI.Linux
{
    internal class GtkWebview : IWebview
    {
        public event EventHandler CloseRequested;
        public event EventHandler<string> TitleChanged;

        public ScriptHandler ScriptHandler { get; }

        public readonly IntPtr Handle;

        private readonly IntPtr manager;

        public GtkWebview(bool enableScriptInterface)
        {
            if (enableScriptInterface)
            {
                ScriptHandler = new ScriptHandler(this);

                manager = WebKit.Manager.Create();
                using (GLibString name = "script-message-received::external")
                {
                    GLib.ConnectSignal(manager, name, (ScriptDelegate)ScriptCallback, IntPtr.Zero);
                }

                using (GLibString name = "external")
                {
                    WebKit.Manager.RegisterScriptMessageHandler(manager, name);
                }

                using (GLibString scriptText = Resources.GetInitScript("Linux"))
                {
                    IntPtr script = IntPtr.Zero;

                    try
                    {
                        script = WebKit.Manager.CreateScript(
                            manager,
                            scriptText,
                            WebKitInjectedFrames.AllFrames,
                            WebKitInjectionTime.DocumentStart,
                            IntPtr.Zero,
                            IntPtr.Zero);

                        WebKit.Manager.AddScript(manager, script);
                    }
                    finally { WebKit.Manager.UnrefScript(script); }
                }

                Handle = WebKit.CreateWithUserContentManager(manager);
            }
            else { Handle = WebKit.Create(); }

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

        public string ExecuteScript(string script)
        {
            return ExecuteScriptAsync(script).GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            Gtk.Widget.Destroy(Handle);
        }

        public async Task<string> ExecuteScriptAsync(string script)
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

                using (GLibString gfunction = script)
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
