using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SpiderEye.Bridge;
using SpiderEye.Configuration;
using SpiderEye.Content;
using SpiderEye.Tools;
using SpiderEye.UI.Linux.Interop;
using SpiderEye.UI.Linux.Native;

namespace SpiderEye.UI.Linux
{
    internal class GtkWebview : IWebview
    {
        public event EventHandler PageLoaded;
        public event EventHandler CloseRequested;
        public event EventHandler<string> TitleChanged;

        public readonly IntPtr Handle;

        private readonly IContentProvider contentProvider;
        private readonly AppConfiguration config;
        private readonly WebviewBridge bridge;
        private readonly IntPtr manager;
        private readonly string customHost;

        public GtkWebview(AppConfiguration config, IContentProvider contentProvider, WebviewBridge bridge)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.contentProvider = contentProvider ?? throw new ArgumentNullException(nameof(contentProvider));
            this.bridge = bridge ?? throw new ArgumentNullException(nameof(bridge));

            if (config.EnableScriptInterface)
            {
                manager = WebKit.Manager.Create();
                GLib.ConnectSignal(manager, "script-message-received::external", (ScriptDelegate)ScriptCallback, IntPtr.Zero);

                using (GLibString name = "external")
                {
                    WebKit.Manager.RegisterScriptMessageHandler(manager, name);
                }

                using (GLibString scriptText = Resources.GetInitScript("Unix"))
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

            GLib.ConnectSignal(Handle, "load-changed", (PageLoadDelegate)LoadCallback, IntPtr.Zero);
            GLib.ConnectSignal(Handle, "context-menu", (ContextMenuRequestDelegate)ContextMenuCallback, IntPtr.Zero);
            GLib.ConnectSignal(Handle, "close", (WebviewDelegate)CloseCallback, IntPtr.Zero);

            if (config.Window.UseBrowserTitle)
            {
                GLib.ConnectSignal(Handle, "notify::title", (WebviewDelegate)TitleChangeCallback, IntPtr.Zero);
            }

            if (string.IsNullOrWhiteSpace(config.ExternalHost))
            {
                const string scheme = "spidereye";
                customHost = UriTools.GetRandomResourceUrl(scheme);

                IntPtr context = WebKit.Context.Get(Handle);
                using (GLibString gscheme = scheme)
                {
                    WebKit.Context.RegisterUriScheme(context, gscheme, UriSchemeCallback, IntPtr.Zero, IntPtr.Zero);
                }
            }
        }

        public void NavigateToFile(string url)
        {
            if (url == null) { throw new ArgumentNullException(nameof(url)); }

            if (customHost != null) { url = UriTools.Combine(customHost, url).ToString(); }
            else { url = UriTools.Combine(config.ExternalHost, url).ToString(); }

            using (GLibString gurl = url)
            {
                WebKit.LoadUri(Handle, gurl);
            }
        }

        public string ExecuteScript(string script)
        {
            var task = ExecuteScriptAsync(script);

            // main loop would deadlock without this
            while (!task.IsCompleted)
            {
                Gtk.MainIteration();
            }

            return task.Result;
        }

        public Task<string> ExecuteScriptAsync(string script)
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
                            taskResult.TrySetException(new Exception($"Script execution failed with: \"{errorMessage}\""));
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
        }

        public void Dispose()
        {
            // gets automatically disposed by parent window
        }

        private async void ScriptCallback(IntPtr manager, IntPtr jsResult, IntPtr userdata)
        {
            IntPtr value = WebKit.JavaScript.GetValue(jsResult);
            if (WebKit.JavaScript.IsValueString(value))
            {
                IntPtr bytes = IntPtr.Zero;
                try
                {
                    bytes = WebKit.JavaScript.GetStringBytes(value);
                    IntPtr bytesPtr = GLib.GetBytesDataPointer(bytes, out UIntPtr length);

                    string result;
                    unsafe { result = Encoding.UTF8.GetString((byte*)bytesPtr, (int)length); }

                    await bridge.HandleScriptCall(result);
                }
                finally { if (bytes != IntPtr.Zero) { GLib.UnrefBytes(bytes); } }
            }
        }

        private async void UriSchemeCallback(IntPtr request, IntPtr userdata)
        {
            try
            {
                var uri = new Uri(GLibString.FromPointer(WebKit.UriScheme.GetRequestUri(request)));
                string schemeAndServer = uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped);
                if (schemeAndServer == customHost)
                {
                    using (var contentStream = await contentProvider.GetStreamAsync(uri))
                    {
                        if (contentStream != null)
                        {
                            IntPtr stream = IntPtr.Zero;
                            try
                            {
                                if (contentStream is UnmanagedMemoryStream unmanagedMemoryStream)
                                {
                                    unsafe
                                    {
                                        long length = unmanagedMemoryStream.Length - unmanagedMemoryStream.Position;
                                        stream = GLib.CreateStreamFromData((IntPtr)unmanagedMemoryStream.PositionPointer, length, IntPtr.Zero);
                                        FinishUriSchemeCallback(request, stream, length, uri);
                                        return;
                                    }
                                }
                                else
                                {
                                    byte[] data;
                                    long length;
                                    if (contentStream is MemoryStream memoryStream)
                                    {
                                        data = memoryStream.GetBuffer();
                                        length = memoryStream.Length;
                                    }
                                    else
                                    {
                                        using (var copyStream = new MemoryStream())
                                        {
                                            await contentStream.CopyToAsync(copyStream);
                                            data = copyStream.GetBuffer();
                                            length = copyStream.Length;
                                        }
                                    }

                                    unsafe
                                    {
                                        fixed (byte* dataPtr = data)
                                        {
                                            stream = GLib.CreateStreamFromData((IntPtr)dataPtr, length, IntPtr.Zero);
                                            FinishUriSchemeCallback(request, stream, length, uri);
                                            return;
                                        }
                                    }
                                }
                            }
                            finally { if (stream != IntPtr.Zero) { GLib.UnrefObject(stream); } }
                        }
                    }
                }

                FinishUriSchemeCallbackWithError(request);
            }
            catch { FinishUriSchemeCallbackWithError(request); }
        }

        private void LoadCallback(IntPtr webview, WebKitLoadEvent type, IntPtr userdata)
        {
            if (type == WebKitLoadEvent.Finished)
            {
                PageLoaded?.Invoke(this, EventArgs.Empty);
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

        private void FinishUriSchemeCallback(IntPtr request, IntPtr stream, long streamLength, Uri uri)
        {
            using (GLibString mimetype = MimeTypes.FindForUri(uri))
            {
                WebKit.UriScheme.FinishSchemeRequest(request, stream, streamLength, mimetype);
            }
        }

        private void FinishUriSchemeCallbackWithError(IntPtr request)
        {
            uint domain = GLib.GetFileErrorQuark();
            var error = new GError(domain, 4, IntPtr.Zero); // error code 4 = not found
            WebKit.UriScheme.FinishSchemeRequestWithError(request, ref error);
        }
    }
}
