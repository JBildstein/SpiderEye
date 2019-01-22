using System;
using System.Runtime.InteropServices;
using SpiderEye.UI.Linux.Interop;

namespace SpiderEye.UI.Linux.Native
{
    internal static class WebKit
    {
        private const string WebkitNativeDll = "libwebkit2gtk-4.0.so";
        private const string JavaScriptCoreNativeDll = "libjavascriptcoregtk-4.0.so";

        public static class Manager
        {
            [DllImport(WebkitNativeDll, EntryPoint = "webkit_user_content_manager_new", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr Create();

            [DllImport(WebkitNativeDll, EntryPoint = "webkit_user_content_manager_register_script_message_handler", CallingConvention = CallingConvention.Cdecl)]
            public static extern bool RegisterScriptMessageHandler(IntPtr manager, IntPtr name);

            [DllImport(WebkitNativeDll, EntryPoint = "webkit_user_script_new", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr CreateScript(IntPtr manager, IntPtr source, WebKitInjectedFrames injectedFrames, WebKitInjectionTime injectedTime, IntPtr whitelist, IntPtr blacklist);

            [DllImport(WebkitNativeDll, EntryPoint = "webkit_user_content_manager_add_script", CallingConvention = CallingConvention.Cdecl)]
            public static extern void AddScript(IntPtr manager, IntPtr script);

            [DllImport(WebkitNativeDll, EntryPoint = "webkit_user_script_unref", CallingConvention = CallingConvention.Cdecl)]
            public static extern bool UnrefScript(IntPtr script);
        }

        public static class Settings
        {
            [DllImport(WebkitNativeDll, EntryPoint = "webkit_web_view_get_settings", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr Get(IntPtr webview);

            [DllImport(WebkitNativeDll, EntryPoint = "webkit_settings_set_enable_write_console_messages_to_stdout", CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetEnableWriteConsoleMessagesToStdout(IntPtr settings, bool enabled);

            [DllImport(WebkitNativeDll, EntryPoint = "webkit_settings_set_enable_developer_extras", CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetEnableDeveloperExtras(IntPtr settings, bool enabled);
        }

        public static class Inspector
        {
            [DllImport(WebkitNativeDll, EntryPoint = "webkit_web_view_get_inspector", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr Get(IntPtr webview);

            [DllImport(WebkitNativeDll, EntryPoint = "webkit_web_inspector_show", CallingConvention = CallingConvention.Cdecl)]
            public static extern void Show(IntPtr inspector);
        }

        public static class Context
        {
            [DllImport(WebkitNativeDll, EntryPoint = "webkit_web_view_get_context", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr Get(IntPtr webview);

            [DllImport(WebkitNativeDll, EntryPoint = "webkit_web_context_register_uri_scheme", CallingConvention = CallingConvention.Cdecl)]
            public static extern void RegisterUriScheme(IntPtr context, IntPtr scheme, WebKitUriSchemeRequestDelegate callback, IntPtr user_data, IntPtr user_data_destroy_func);
        }

        public static class UriScheme
        {
            [DllImport(WebkitNativeDll, EntryPoint = "webkit_uri_scheme_request_finish", CallingConvention = CallingConvention.Cdecl)]
            public static extern void FinishSchemeRequest(IntPtr request, IntPtr stream, long stream_length, IntPtr mime_type);

            [DllImport(WebkitNativeDll, EntryPoint = "webkit_uri_scheme_request_finish_error", CallingConvention = CallingConvention.Cdecl)]
            public static extern void FinishSchemeRequestWithError(IntPtr request, ref GError error);

            [DllImport(WebkitNativeDll, EntryPoint = "webkit_uri_scheme_request_get_path", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr GetRequestPath(IntPtr request);

            [DllImport(WebkitNativeDll, EntryPoint = "webkit_uri_scheme_request_get_uri", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr GetRequestUri(IntPtr request);
        }

        public static class UriRequest
        {
            [DllImport(WebkitNativeDll, EntryPoint = "webkit_uri_request_get_uri", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr GetRequestUri(IntPtr request);

            [DllImport(WebkitNativeDll, EntryPoint = "webkit_uri_request_set_uri", CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetRequestUri(IntPtr request, IntPtr uri);
        }

        public static class JavaScript
        {
            [DllImport(WebkitNativeDll, EntryPoint = "webkit_web_view_run_javascript", CallingConvention = CallingConvention.Cdecl)]
            public static extern void BeginExecute(IntPtr webview, IntPtr js, IntPtr cancellable, GAsyncReadyDelegate callback, IntPtr user_data);

            [DllImport(WebkitNativeDll, EntryPoint = "webkit_web_view_run_javascript_finish", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr EndExecute(IntPtr webview, IntPtr asyncResult, out IntPtr error);

            [DllImport(WebkitNativeDll, EntryPoint = "webkit_javascript_result_get_js_value", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr GetValue(IntPtr jsResult);

            [DllImport(JavaScriptCoreNativeDll, EntryPoint = "jsc_value_is_string", CallingConvention = CallingConvention.Cdecl)]
            public static extern bool IsValueString(IntPtr value);

            [DllImport(JavaScriptCoreNativeDll, EntryPoint = "jsc_value_to_string_as_bytes", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr GetStringBytes(IntPtr value);

            [DllImport(WebkitNativeDll, EntryPoint = "webkit_javascript_result_unref", CallingConvention = CallingConvention.Cdecl)]
            public static extern void ReleaseJsResult(IntPtr jsResult);
        }

        [DllImport(WebkitNativeDll, EntryPoint = "webkit_web_view_new", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Create();

        [DllImport(WebkitNativeDll, EntryPoint = "webkit_web_view_new_with_user_content_manager", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CreateWithUserContentManager(IntPtr manager);

        [DllImport(WebkitNativeDll, EntryPoint = "webkit_web_view_load_uri", CallingConvention = CallingConvention.Cdecl)]
        public static extern void LoadUri(IntPtr webview, IntPtr uri);

        [DllImport(WebkitNativeDll, EntryPoint = "webkit_web_view_set_background_color", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetBackgroundColor(IntPtr webview, ref GdkColor color);

        [DllImport(WebkitNativeDll, EntryPoint = "webkit_web_view_get_title", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetTitle(IntPtr webview);
    }
}
